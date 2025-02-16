using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Users.Events
{
    public record CleanCacheForUserRequestSent
    {
        public Guid UserId { get; init; } = default!;
        public Guid? TenantId { get; init; } = default!;
        public string AuthId { get; init; } = default!;
    }
}
