using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using UbikLink.Common.Api;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.Proxy.Services
{
    public sealed class UserService(HttpClient httpClient, HybridCache cache, IOptions<ProxyToken> proxyToken)
    {
        public async Task<UserProxyResult?> GetUserInfoAsync(string? oauthId)
        {
            var secuToken = proxyToken.Value.Token;

            if (oauthId == null) return null;

            var user = await cache.GetOrCreateAsync<UserProxyResult?>($"proxy_{oauthId}", factory: null!,
            new HybridCacheEntryOptions() { Flags = HybridCacheEntryFlags.DisableUnderlyingData });

            if (user == null)
            {
                httpClient.DefaultRequestHeaders.Add("x-proxy-token", secuToken);
                var response = await httpClient.GetAsync($"/api/v1/proxy/users?authid={oauthId}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var userInfo = await response.Content.ReadFromJsonAsync<UserProxyResult>();

                        if (userInfo == null)
                            return null;
                        else
                        {
                            await SetUserInfoInCacheAsync(userInfo);
                            return userInfo;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                //Cache
                return user;
            }
        }

        private async Task SetUserInfoInCacheAsync(UserProxyResult userInfo)
        {
            var tagsList = new List<string>
            {
                $"proxy_UserInfo",
                $"proxy_UserInfo_{userInfo.SelectedTenantId}"
            };

            await cache.SetAsync($"proxy_{userInfo.AuthId}", userInfo, new HybridCacheEntryOptions()
            {
                Expiration = TimeSpan.FromMinutes(15),
                LocalCacheExpiration = TimeSpan.Zero //Disable local cache (cache entry can be revoked from msg service)
            },
            tagsList);
        }

        public async Task RemoveUserInfoFromCacheAsync(string oAuthId)
        {
            await cache.RemoveAsync($"proxy_{oAuthId}");
        }

        public async Task RemoveAllUserInfoFromCache()
        {
            await cache.RemoveByTagAsync("proxy_UserInfo");
        }

        public async Task RemoveAllUserInfoFromCache(Guid tenantId)
        {
            await cache.RemoveByTagAsync($"proxy_UserInfo_{tenantId}");
        }
    }
}
