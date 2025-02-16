using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Roles.Events
{
    public record CleanCacheRoleUpdated
    {
        public Guid RoleId { get; init; } = default!;
        public Guid? TenantId { get; init; } = null;
    }
}
