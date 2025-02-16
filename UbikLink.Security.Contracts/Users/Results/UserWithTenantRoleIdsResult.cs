using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Roles.Results;

namespace UbikLink.Security.Contracts.Users.Results
{
    public record UserWithTenantRoleIdsResult
    {
        public Guid Id { get; init; }
        public required string Firstname { get; init; }
        public required string Lastname { get; init; }
        public required string Email { get; init; }
        public List<Guid> TenantRoleIds { get; init; } = [];
        public Guid Version { get; init; }
    }
}
