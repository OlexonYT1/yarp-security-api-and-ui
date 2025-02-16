using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Common.Auth;

namespace UbikLink.Common.Frontend.Auth
{
    public static class AddAuthForClientUIExtension
    {
        public static IServiceCollection AddAuthForClientUI(this IServiceCollection services, AuthConfigOptions authOptions)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(authOptions.CookieRefreshTimeInMinutes);
                options.SlidingExpiration = true;

                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = async x =>
                    {
                        var now = DateTimeOffset.UtcNow;
                        var userId = x.Principal!.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                        //Try to get user in cache
                        var cache = x.HttpContext.RequestServices.GetRequiredService<UserAndTokenCache>();
                        var actualToken = await cache.GetUserTokenAsync(userId);

                        //If no token
                        if (actualToken == null)
                        {
                            x.RejectPrincipal();
                            return;
                        }
                    }
                };
            })
            .AddOpenIdConnect(options =>
            {
                {
                    options.Authority = authOptions.Authority;
                    options.MetadataAddress = authOptions.MetadataAddress;
                    options.ClientSecret = authOptions.ClientSecret;
                    options.ClientId = authOptions.ClientId;
                    options.ResponseType = "code";
                    options.SaveTokens = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    
                    options.Scope.Clear();
                    foreach(var scope in authOptions.Scopes)
                    {
                        options.Scope.Add(scope);
                    }

                    options.RequireHttpsMetadata = authOptions.RequireHttpsMetadata;

                    //Uncomment if really needed
                    //if (authOptions.AuthorizeBadCert)
                    //{
                    //    //TODO; remove that shit on prod... only for DEV keycloak Minikube
                    //    HttpClientHandler handler = new()
                    //    {
                    //        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    //    };
                    //    options.BackchannelHttpHandler = handler;
                    //}

                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = context => {
                            if(!string.IsNullOrEmpty(authOptions.Audience))
                                context.ProtocolMessage.SetParameter("audience", authOptions.Audience);

                            return Task.CompletedTask;
                        },
                        //Store token in cache
                        OnTokenValidated = async x =>
                        {
                            var now = DateTimeOffset.UtcNow;
                            var cache = x.HttpContext.RequestServices.GetRequiredService<UserAndTokenCache>();
                            var token = new TokenCacheEntry
                            {
                                UserId = x.Principal!.FindFirst(ClaimTypes.NameIdentifier)!.Value,
                                AccessToken = x.TokenEndpointResponse!.AccessToken,
                                RefreshToken = x.TokenEndpointResponse.RefreshToken,
                                ExpiresUtc = now.AddMinutes(authOptions.AccessTokenExpTimeInMinutes-1),
                                ExpiresRefreshUtc = now.AddMinutes(authOptions.RefreshTokenExpTimeInMinutes-1)
                            };
                            x.Properties!.IsPersistent = true;
                            x.Properties.ExpiresUtc = now.AddMinutes(authOptions.RefreshTokenExpTimeInMinutes - 1);

                            await cache.SetUserTokenAsync(token);
                            x.Success();
                        },
                        //Only store the Id token for more security
                        OnTokenResponseReceived = async x =>
                        {
                            ////Only store id_token in cookie
                            x.Properties!.StoreTokens([ new AuthenticationToken
                                {
                                    Name = "id_token",
                                    Value = x.TokenEndpointResponse.IdToken
                                }]);

                            await Task.CompletedTask;
                        },
                    };
                }
            });

            return services;
        }
    }
}
