using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Common.Frontend.Auth
{
    public class TokenCacheEntry
    {
        public string UserId { get; set; } = default!;
        public string IdToken { get; set; } = default!;
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTimeOffset ExpiresUtc { get; set; } = default!;
        public DateTimeOffset ExpiresRefreshUtc { get; set; } = default!;
    }
}
