using Microsoft.AspNetCore.Components;

namespace UbikLink.Security.UI.Client.Components.Subscription
{
    public partial class SubscriptionInfo
    {
        [Parameter]
        public bool IsMainLoading { get; set; } = false;

        [Parameter]
        public SubscriptionUiObj? Subscription { get; set; }
    }
}
