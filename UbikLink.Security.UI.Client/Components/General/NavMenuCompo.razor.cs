using Microsoft.AspNetCore.Components;

namespace UbikLink.Security.UI.Client.Components.General
{
    public partial class NavMenuCompo
    {
        [Parameter]
        public bool IsMegaAdmin { get; set; } = false;

        [Parameter]
        public bool IsSubscriptionOwner { get; set; } = false;

        [Parameter]
        public bool IsTenantManager { get; set; } = false;

        private bool expanded = true;
    }
}
