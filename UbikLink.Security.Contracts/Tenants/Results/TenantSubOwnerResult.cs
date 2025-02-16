using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Tenants.Results
{
    public record TenantSubOwnerResult
    {
        public Guid Id { get; init; }
        public required Guid SubscriptionId { get; init; }
        public required string Label { get; init; }
        public bool IsActivated { get; init; } = true;
        public List<Guid> LinkedUserIds { get; init; } = [];
        public Guid Version { get; init; }
    }
}
