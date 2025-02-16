using Microsoft.AspNetCore.Components;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Roles.Results;
using UbikLink.Security.Contracts.Subscriptions.Results;
using UbikLink.Security.Contracts.Tenants.Results;
using UbikLink.Security.Contracts.Users.Results;
using UbikLink.Security.UI.Shared.Httpclients;

namespace UbikLink.Security.UI.Client.Components.Tenant
{
    public partial class Tenant(IHttpSecurityClient securityClient,
        ResponseManagerService responseManager)
    {
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly ResponseManagerService _responseManager = responseManager;

        private bool _isLoading = false;

        private UserMeResult? _connectedUserInfo;
        private TenantStandardResult? _selectedTenant;
        private List<UserWithTenantRoleIdsResult>? _tenantUsers;
        private List<RoleLightResult>? _roles;

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

            await LoadUserInfoAsync();

            var loadTasks = new List<Task>
            {
                LoadSelectedTenantAsync((Guid)_connectedUserInfo!.SelectedTenantId!),
                LoadTenantUsersAsync((Guid)_connectedUserInfo!.SelectedTenantId!),
                LoadTenantAvailableRolesAsync((Guid)_connectedUserInfo!.SelectedTenantId!)
            };

            await Task.WhenAll(loadTasks);

            _isLoading = false;
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

        private async Task LoadSelectedTenantAsync(Guid tenantId)
        {
            var response = await _securityClient.GetTenantAsync(tenantId);
            var result = await _responseManager.ManageAsync<TenantStandardResult>(
                response,
                "Cannot retrieve data.",
                "An error occurred while loading the tenant.");

            if (result != null)
                _selectedTenant = result;
        }

        private async Task LoadTenantUsersAsync(Guid tenantId)
        {
            var response = await _securityClient.GetTenantUsersWithRolesIdsAsync(tenantId);
            var result = await _responseManager.ManageAsync<List<UserWithTenantRoleIdsResult>>(
                response,
                "Cannot retrieve data.",
                "An error occurred while loading the users.");

            if (result != null)
                _tenantUsers = result;
        }

        private async Task LoadTenantAvailableRolesAsync(Guid tenantId)
        {
            var response = await _securityClient.GetTenantAvailableRolesAsync(tenantId);
            var result = await _responseManager.ManageAsync<List<RoleLightResult>>(
                response,
                "Cannot retrieve data.",
                "An error occurred while loading the roles.");

            if (result != null)
                _roles = result;
        }
    }
}
