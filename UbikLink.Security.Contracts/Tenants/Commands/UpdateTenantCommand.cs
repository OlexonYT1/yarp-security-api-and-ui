using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Tenants.Commands
{
    public record UpdateTenantCommand
    {
        public required string Label { get; init; }
        public required Guid SubscriptionId { get; init; }
        public required bool IsActivated { get; init; }
        public required Guid Version { get; init; }
    }
}
