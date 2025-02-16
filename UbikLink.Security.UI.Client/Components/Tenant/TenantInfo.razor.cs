using Microsoft.AspNetCore.Components;
using UbikLink.Security.Contracts.Tenants.Results;
using UbikLink.Security.UI.Client.Components.Subscription;

namespace UbikLink.Security.UI.Client.Components.Tenant
{
    public partial class TenantInfo
    {
        [Parameter]
        public bool IsMainLoading { get; set; } = false;

        [Parameter]
        public TenantStandardResult? Tenant { get; set; }
    }
}
