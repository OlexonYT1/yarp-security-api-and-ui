using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using UbikLink.Common.Frontend.Auth;
using UbikLink.Common.RazorUI.Components;
using UbikLink.Security.UI.Shared.Httpclients;

namespace UbikLink.Security.UI.Components.Subscription
{
    public partial class SubscriptionPage(ClientUserService userService, IHttpSecurityClient securityClient)
        : Basepage(userService, securityClient)
    {
        protected override List<BreadcrumbListItem> BreadcrumbItems
        {
            get
            {
                return _breadcrumbItems;
            }
        }

        private bool _isSubscriptionOwner = false;

        private static readonly List<BreadcrumbListItem> _breadcrumbItems = [
            new BreadcrumbListItem()
            {
                Position =1,
                Url = "/",
                Name = "Home"
            }];

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var (IsOwner, _) = await IsOwnerOfTheSelectedSubscription();

            _isSubscriptionOwner = IsOwner;
        }
    }
}
