using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Security.Contracts.Tenants.Events
{
    public record CleanCacheTenantDeleted
    {
        public Guid TenantId { get; init; } = default!;
    }
}
