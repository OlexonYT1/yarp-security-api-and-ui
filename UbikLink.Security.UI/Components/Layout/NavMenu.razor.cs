using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using UbikLink.Common.Frontend.Auth;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.Security.UI.Components.Layout
{
    public partial class NavMenu(ClientUserService userService)
    {
        private readonly ClientUserService _userService = userService;

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        private bool _isMegaAdmin = false;
        private bool _isSubscriptionOwner = false;
        private bool _isTenantManager = false;  

        protected override async Task OnInitializedAsync()
        {

            if (AuthenticationState is not null)
            {
                await AuthenticationState;
                var user = await _userService.GetUserInfoAsync();

                if (user is not null)
                {
                    _isMegaAdmin = user.IsMegaAdmin;

                    if (user.OwnerOfSubscriptionsIds.Any())
                        _isSubscriptionOwner = true;
                    
                    if(CanAccessIfHasAuthorizations(user,["tenant:read", "user:read", "tenant-user-role:write", "tenant-role:read"]))
                        _isTenantManager = true;
                }
            }
        }

        private bool CanAccessIfHasAuthorizations(UserMeResult user, params string[] authCodes)
        {
            return user.IsActivatedInSelectedSubscription
                && (authCodes.Length == 0
                    || !authCodes.Except(user.SelectedTenantAuthorizations.Select(x=>x.Code)).Any());
        }
    }
}
