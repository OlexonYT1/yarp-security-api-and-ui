using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Subscriptions.Results
{
    public record SubscriptionStandardResult
    {
        public Guid Id { get; init; }
        public string Label { get; init; } = default!;
        public bool IsActive { get; init; } = true;
        public Guid Version { get; init; }
    }
}
