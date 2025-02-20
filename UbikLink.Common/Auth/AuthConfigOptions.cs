using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Common.Auth
{
    public class AuthConfigOptions
    {
        public const string Position = "AuthConfig";
        public string MetadataAddress { get; set; } = string.Empty;
        public string Authority { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public bool RequireHttpsMetadata { get; set; } = true;
        public string AuthorizationUrl { get; set; } = string.Empty;
        public string TokenUrl { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public int CookieRefreshTimeInMinutes { get; set; } = 0;
        public int AccessTokenExpTimeInMinutes { get; set; } = 0;
        public int RefreshTokenExpTimeInMinutes { get; set; } = 25;
        public string ClientAppName { get; set; } = string.Empty;
        public List<string> Scopes { get; set; } = default!;
        public bool AuthorizeBadCert { get; set; } = false;
        public string AuthTokenHttpClientName { get; set; } = "default";
        public string AuthTokenStoreKey { get; set; } = "default";
    }
}
