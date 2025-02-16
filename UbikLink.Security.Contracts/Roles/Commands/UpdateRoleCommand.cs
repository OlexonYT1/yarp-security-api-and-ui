using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Roles.Commands
{
    public record UpdateRoleCommand
    {
        public required string Code { get; init; }
        public string? Description { get; init; }
        public Guid? TenantId { get; init; } // A role can be a general role and a specific role for a tenant
        public List<Guid> AuthorizationIds { get; init; } = [];
        public Guid Version { get; init; }
    }
}
