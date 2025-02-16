using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Roles.Commands
{
    public record BatchDeleteRoleCommand
    {
        public List<Guid> RoleIds { get; init; } = [];
    }
}
