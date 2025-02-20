using Duende.IdentityModel.Client;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using UbikLink.Common.Auth;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.Common.Frontend.Auth
{
    public class UserAndTokenCache(HybridCache cache, IOptions<AuthConfigOptions> authOptions, IHttpClientFactory factory)
    {
        private readonly static int _localCacheExpirationInMinutes = 10;
        private readonly AuthConfigOptions _authOptions = authOptions.Value;
        private readonly HttpClient _httpClient = factory.CreateClient(authOptions.Value.AuthTokenHttpClientName);

        public async Task RemoveUserTokenAsync(string key)
        {
            await cache.RemoveAsync($"{_authOptions.ClientAppName}_{key}");
        }

        public async Task SetUserTokenAsync(TokenCacheEntry token)
        {
            token.AccessToken = Encrypt(token.AccessToken, _authOptions.AuthTokenStoreKey);
            token.RefreshToken = Encrypt(token.RefreshToken, _authOptions.AuthTokenStoreKey);
            await cache.SetAsync($"{_authOptions.ClientAppName}_{token.UserId}", token, new HybridCacheEntryOptions()
            {
                Expiration = TimeSpan.FromMinutes(_authOptions.RefreshTokenExpTimeInMinutes + 1),
                LocalCacheExpiration = TimeSpan.FromMinutes(_localCacheExpirationInMinutes),
            });
        }

        public async Task<TokenCacheEntry?> GetUserTokenAsync(string? userId)
        {
            if (userId == null) return null;

            var token = await cache.GetOrCreateAsync<TokenCacheEntry?>($"{_authOptions.ClientAppName}_{userId}", factory: null!,
                    new HybridCacheEntryOptions() { Flags = HybridCacheEntryFlags.DisableUnderlyingData });

            if (token == null) return null;

            token.AccessToken = Decrypt(token.AccessToken, _authOptions.AuthTokenStoreKey);
            token.RefreshToken = Decrypt(token.RefreshToken, _authOptions.AuthTokenStoreKey);
            token = await RefreshTokenAsync(token, userId);

            return token;
        }

        public async Task SetUserInfoAsync(UserMeResult userInfo)
        {
            var tagsList = new List<string> 
            { 
                $"{_authOptions.ClientAppName}_UserInfo",
                $"{_authOptions.ClientAppName}_UserInfo_{userInfo.SelectedTenantId}"
            };

            await cache.SetAsync($"{_authOptions.ClientAppName}_{userInfo.AuthId}_me_info", 
                userInfo,
                new HybridCacheEntryOptions()
                    {
                        Expiration = TimeSpan.FromMinutes(_authOptions.CookieRefreshTimeInMinutes + 2),
                        LocalCacheExpiration = TimeSpan.Zero //Disable local cache (cache entry can be revoked from msg service)
                },
                tagsList);
        }

        public async Task<UserMeResult?> GetUserInfoAsync(string? userAuthId)
        {
            if (userAuthId == null) return null;

            var user = await cache.GetOrCreateAsync<UserMeResult?>($"{_authOptions.ClientAppName}_{userAuthId}_me_info", 
                factory: null!,
                new HybridCacheEntryOptions() { Flags = HybridCacheEntryFlags.DisableUnderlyingData });

            return user ?? null;
        }

        private async Task<TokenCacheEntry?> RefreshTokenAsync(TokenCacheEntry actualToken, string userId)
        {

            if (actualToken.ExpiresUtc > DateTimeOffset.UtcNow.AddSeconds(10))
            {
                //No need to refresh
                return actualToken;
            }
            else
            {
                if (actualToken.ExpiresRefreshUtc > DateTimeOffset.UtcNow.AddSeconds(10))
                {
                    //Can try a refresh
                    var dict = ValuesForRefresh(actualToken.RefreshToken);
                    HttpResponseMessage response = await _httpClient.PostAsync("", new FormUrlEncodedContent(dict));

                    if (response.IsSuccessStatusCode)
                    {
                        var token = await ProtocolResponse.FromHttpResponseAsync<TokenResponse>(response);

                        if (token != null)
                        {
                            var newToken = new TokenCacheEntry
                            {
                                UserId = userId,
                                RefreshToken = token.RefreshToken!,
                                AccessToken = token.AccessToken!,
                                ExpiresUtc = new JwtSecurityToken(token.AccessToken).ValidTo,
                                ExpiresRefreshUtc = DateTimeOffset.UtcNow.AddMinutes(_authOptions.RefreshTokenExpTimeInMinutes)
                            };

                            //Refresh successful
                            await SetUserTokenAsync(newToken);
                            return newToken;
                        }
                    }
                }
                //Too old to refresh or refresh not successful
                await RemoveUserTokenAsync(userId);
                return null;
            }
        }

        public async Task RemoveUserInfoInCacheAsync(string userAuthId)
        {
            await cache.RemoveAsync($"{_authOptions.ClientAppName}_{userAuthId}_me_info");
        }

        public async Task RemoveAllUserInfoInCacheAsync()
        {
            await cache.RemoveByTagAsync($"{_authOptions.ClientAppName}_UserInfo");
        }

        public async Task RemoveAllUserInfoInCacheForTenantAsync(Guid tenantId)
        {
            await cache.RemoveByTagAsync($"{_authOptions.ClientAppName}_UserInfo_{tenantId}");
        }

        private Dictionary<string, string> ValuesForRefresh(string token)
        {
            return new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                { "client_id", _authOptions.ClientId },
                { "client_secret", _authOptions.ClientSecret },
                { "refresh_token", token },
                { "grant_type", "refresh_token" },
            };
        }

        public static string Encrypt(string plainText, string key)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.GenerateIV();
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new();
            msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
            using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public static string Decrypt(string cipherText, string key)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            byte[] iv = new byte[aesAlg.BlockSize / 8];
            byte[] cipher = new byte[fullCipher.Length - iv.Length];

            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aesAlg.IV = iv;
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new(cipher);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }
}
