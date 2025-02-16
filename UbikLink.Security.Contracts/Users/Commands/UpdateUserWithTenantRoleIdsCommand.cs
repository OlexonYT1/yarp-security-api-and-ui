using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Users.Commands
{
    public record UpdateUserWithTenantRoleIdsCommand
    {
        public List<Guid> TenantRoleIds { get; init; } = [];
    }
}
