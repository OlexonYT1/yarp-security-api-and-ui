using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Users.Results
{
    public record UserSubOwnerResult
    {
        public Guid Id { get; init; }
        public required string Firstname { get; init; }
        public required string Lastname { get; init; }
        public required string Email { get; init; }
        public bool IsActivated { get; init; } = true;
        public bool IsSubscriptionOwner { get; init; } = true;
        public IEnumerable<UserLinkedTenantSubOwnerResult> LinkedTenants { get; init; } = [];
        public Guid Version { get; init; }
    }

    public record UserLinkedTenantSubOwnerResult
    {
        public Guid Id { get; init; }
        public required string Label { get; init; }
    }
}
