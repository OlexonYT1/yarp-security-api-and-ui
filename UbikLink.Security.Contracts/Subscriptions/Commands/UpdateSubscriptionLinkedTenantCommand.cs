using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Subscriptions.Commands
{
    public record UpdateSubscriptionLinkedTenantCommand
    {
        public required string Label { get; init; }
        public required Guid SubscriptionId { get; init; }
        public bool Active { get; init; } = true;
        public Guid Version { get; init; } = default!;
        public List<Guid> LinkedUsersIds { get; init; } = [];
    }
}
