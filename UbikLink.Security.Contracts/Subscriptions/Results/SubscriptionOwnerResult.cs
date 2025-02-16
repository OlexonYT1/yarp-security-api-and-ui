using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Subscriptions.Results
{
    public record SubscriptionOwnerResult
    {
        public Guid Id { get; init; }
        public bool IsActive { get; init; } = true;
        public required string Label { get; init; }
        public required string PlanName { get; init; }
        public int MaxUsers { get; init; } = 1;
        public int MaxTenants { get; init; } = 1;
        public Guid Version { get; init; }
    }
}
