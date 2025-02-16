using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using UbikLink.Common.Frontend.Auth;
using UbikLink.Common.RazorUI.Components;
using UbikLink.Security.Contracts.Users.Results;
using MassTransit.Futures.Contracts;
using UbikLink.Security.Contracts.Subscriptions.Results;
using UbikLink.Security.UI.Shared.Httpclients;

namespace UbikLink.Security.UI.Components
{
    public abstract partial class Basepage(ClientUserService userService,
        IHttpSecurityClient securityClient)
    {
        protected readonly ClientUserService UserService = userService;
        protected readonly IHttpSecurityClient SecurityClient = securityClient;

        [CascadingParameter]
        protected Task<AuthenticationState>? AuthenticationState { get; set; }

        protected UserMeResult? User = null;

        private bool _canAccess = false;
        private bool _isActiveInSelectedSubscription = false;

        protected abstract List<BreadcrumbListItem> BreadcrumbItems { get; } 

        protected override async Task OnInitializedAsync()
        {
            if (AuthenticationState is not null)
            {
                await AuthenticationState;
                User = await UserService.GetUserInfoAsync();
                
                if(User != null)
                {
                    _canAccess = true;
                    _isActiveInSelectedSubscription = User.IsActivatedInSelectedSubscription;
                }
                    
            }
        }

        protected bool IsMegaAdmin()
        {
            return _canAccess && User!.IsMegaAdmin;
        }

        //Settings page will even be exluded from the check
        protected bool CanAccessWithoutRoleOrSubscription()
        {
            return _canAccess && !_isActiveInSelectedSubscription;
        }

        protected bool CanAccessIfInRoles(params string[] roleCodes)
        {
            return _canAccess
                && _isActiveInSelectedSubscription
                && (roleCodes.Length == 0
                    || !roleCodes.Except(User!.SelectedTenantRoles.Select(x => x.Code)).Any());
        }

        protected bool CanAccessIfHasAuthorizations(params string[] authCodes)
        {
            return _canAccess && _isActiveInSelectedSubscription
                && (authCodes.Length == 0
                    || !authCodes.Except(User!.SelectedTenantAuthorizations.Select(x => x.Code)).Any());
        }

        protected async Task<(bool IsOwner, Guid? SelectedSubscriptionId)> IsOwnerOfTheSelectedSubscription()
        {
            var response = await SecurityClient.GetSelectedSubscriptionForOwnerAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var result = await response.Content.ReadFromJsonAsync<SubscriptionOwnerResult>();

                    return result == null 
                        ? ((bool IsOwner, Guid? SelectedSubscriptionId))(false,null) 
                        : ((bool IsOwner, Guid? SelectedSubscriptionId))(true, result.Id);
                }
                catch
                {
                    return (false, null);
                }
            }
            return (false,null);
        }
    }
}
