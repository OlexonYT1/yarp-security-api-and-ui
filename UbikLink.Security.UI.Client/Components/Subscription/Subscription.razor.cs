using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Subscriptions.Results;
using UbikLink.Security.Contracts.Tenants.Results;
using UbikLink.Security.Contracts.Users.Results;
using UbikLink.Security.UI.Client.Components.Settings;
using UbikLink.Security.UI.Shared.Httpclients;

namespace UbikLink.Security.UI.Client.Components.Subscription
{
    public partial class Subscription(IHttpSecurityClient securityClient,
        ResponseManagerService responseManager)
    {
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly ResponseManagerService _responseManager = responseManager;

        [SupplyParameterFromQuery(Name = "state")]
        public string? UseState { get; set; }

        private bool _isLoading = false;
        private SubscriptionUiObj? _selectedSubscription;
        private UserMeResult? _connectedUserInfo;

        protected override async Task OnInitializedAsync()
        {
            if (RendererInfo.IsInteractive)
            {
                await LoadAndSetDataAsync();
            }
        }

        private async Task LoadAndSetDataAsync()
        {
            _isLoading = true;

            var baseTasks = new List<Task>
            {
                LoadUserInfoAsync(),
                LoadSelectedSubscriptionAsync()
            };

            await Task.WhenAll(baseTasks);

            var whenSubOkTasks = new List<Task>
            {
                LoadTenantsAsync(),
                LoadUsersAsync()
            };

            await Task.WhenAll(whenSubOkTasks);

            _isLoading = false;
        }

        private async Task LoadTenantsAsync()
        {
            var response = await _securityClient.GetSelectedSubscriptionLinkedTenantsForSubOwnerAsync();
            if (_selectedSubscription != null)
            {
                var result = await _responseManager.ManageAsync<IEnumerable<TenantSubOwnerResult>>(
                    response,
                    "Cannot retrieve data.",
                    "An error occurred while loading tenants.");

                if (result != null)
                    _selectedSubscription.Tenants = [.. result.ToUiObjs().OrderBy(t => t.Label)];
            }
        }

        private async Task LoadUserInfoAsync()
        {
            var response = await _securityClient.GetConnectedUserInfo();
            var result = await _responseManager.ManageAsync<UserMeResult>(
                response,
                "Cannot retrieve data.",
                "An error occurred while loading connected user info.");

            if (result != null)
                _connectedUserInfo = result;
        }

        private async Task LoadSelectedSubscriptionAsync()
        {
            var response = await _securityClient.GetSelectedSubscriptionForOwnerAsync();
            var result = await _responseManager.ManageAsync<SubscriptionOwnerResult>(
                response,
                "Cannot retrieve data.",
                "An error occurred while loading the subscription.");

            if (result != null)
                _selectedSubscription = result.MapToSubscriptionUiObj();
        }

        private async Task LoadUsersAsync()
        {
            if(_selectedSubscription !=null)
            {
                var response = await _securityClient.GetSelectedSubscriptionLinkedUsersForSubOwnerAsync();
                var result = await _responseManager.ManageAsync<IEnumerable<UserSubOwnerResult>>(
                    response,
                    "Cannot retrieve data.",
                    "An error occurred while loading users.");

                if (result != null)
                {
                    var tmp = result.ToUiObjs().OrderBy(t => t.Email).ToList();
                    _selectedSubscription.Users = tmp.AsQueryable();
                }
            }
        }
    }
}
