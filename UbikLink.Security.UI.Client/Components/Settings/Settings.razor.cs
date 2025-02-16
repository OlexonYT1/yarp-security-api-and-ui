using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Net.Http.Json;
using UbikLink.Common.RazorUI.Components;
using UbikLink.Common.RazorUI.Config;
using UbikLink.Common.RazorUI.Errors;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Subscriptions.Results;
using UbikLink.Security.Contracts.Tenants.Results;
using UbikLink.Security.Contracts.Users.Commands;
using UbikLink.Security.Contracts.Users.Results;
using UbikLink.Security.UI.Shared.Httpclients;

namespace UbikLink.Security.UI.Client.Components.Settings
{
    public partial class Settings(IHttpSecurityClient securityClient,
        IMessageService messageService,
        ResponseManagerService responseManager)
    {
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly IMessageService _messageService = messageService;
        private readonly ResponseManagerService _responseManager = responseManager;

        private List<SubscriptionStandardResult>? _subscriptions;
        private List<TenantStandardResult>? _tenants;
        private UserMeResult? _connectedUserInfo;

        [SupplyParameterFromForm]
        private SettingsUiObj SettingsUiObj { get; set; } = new SettingsUiObj();

        private bool _isSaving = false;
        private bool _isLoading = false;

        private TenantStandardResult? _selectedTenant;

        private List<TenantStandardResult>? TenantsForSub
        {
            get
            {
                return SettingsUiObj.SelectedSubscriptionId != null
                    ? (_tenants?.Where(t => t.SubscriptionId == Guid.Parse(SettingsUiObj.SelectedSubscriptionId)).ToList())
                    : null;
            }
        }

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

            var TaskList = new List<Task>
            {
                LoadSubscriptionsAsync(),
                LoadTenantsAsync(),
                LoadUserInfoAsync()
            };

            await Task.WhenAll(TaskList);

            await SetCurrentSelectedInfo();

            _isLoading = false;
        }

        private async Task LoadSubscriptionsAsync()
        {
            var response = await _securityClient.GetAllSubscribtionsForUserAsync();

            var result = await _responseManager.ManageAsync<IEnumerable<SubscriptionStandardResult>>(
                response,
                "Cannot retrieve data.",
                "An error occurred while loading subscriptions.");

            if (result != null)
                _subscriptions = result.ToList();
        }

        private async Task LoadTenantsAsync()
        {
            var response = await _securityClient.GetAllTenantsForUserAsync();

            var result = await _responseManager.ManageAsync<IEnumerable<TenantStandardResult>>(
                response,
                "Cannot retrieve data.",
                "An error occurred while loading tenants.");

            if (result != null)
                _tenants = result.ToList();
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

        private async Task SetCurrentSelectedInfo()
        {
            if (_connectedUserInfo == null || _tenants == null)
                return;

            // Get the selected tenant ID from the connected user info
            var selectedTenantId = _connectedUserInfo.SelectedTenantId;

            // Find the tenant in the list of tenants
            var selectedTenant = _tenants.FirstOrDefault(t => t.Id == selectedTenantId);

            if (selectedTenant != null)
            {
                // Extract the subscription ID from the selected tenant
                var selectedSubscriptionId = selectedTenant.SubscriptionId;

                // Set the _selectedSubscription value
                SettingsUiObj.SelectedSubscriptionId = selectedSubscriptionId.ToString();
                await InvokeAsync(StateHasChanged);

                // Set the _selectedTenantId and _selectedTenant values 
                SettingsUiObj.SelectedTenantId = selectedTenantId.ToString();
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task OnSubscriptionChangedAsync()
        {
            if (SettingsUiObj.SelectedTenantId != null && TenantsForSub != null)
            {
                if (!TenantsForSub.Any(t => t.Id == Guid.Parse(SettingsUiObj.SelectedTenantId)))
                {
                    SettingsUiObj.SelectedTenantId = TenantsForSub.First().Id.ToString();
                }
            }

            await InvokeAsync(StateHasChanged);
        }

        private async Task HandleValidSubmitAsync()
        {
            _isSaving = true;

            if (_selectedTenant != null)
            {
                var command = new SetSettingsUserMeCommand()
                {
                    TenantId = _selectedTenant.Id
                };

                var response = await _securityClient.SetUserSettings(command);

                await _responseManager.ManageWithSuccessMsgAsync(
                    response,
                    "Save success.",
                    "Settings updated.",
                    "Cannot saving data.",
                    "An error occurred while saving user settings."
                    );
            }

            _isSaving = false;
        }
    }
}
