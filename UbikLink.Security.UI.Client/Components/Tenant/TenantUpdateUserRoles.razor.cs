using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Roles.Results;
using UbikLink.Security.Contracts.Users.Results;
using UbikLink.Security.UI.Client.Components.Roles;
using UbikLink.Security.UI.Client.Components.Subscription;
using UbikLink.Security.UI.Shared.Httpclients;

namespace UbikLink.Security.UI.Client.Components.Tenant
{
    public partial class TenantUpdateUserRoles(IHttpSecurityClient securityClient,
        ResponseManagerService responseManager,
        NavigationManager navigationManager)
    {
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly ResponseManagerService _responseManager = responseManager;
        private readonly NavigationManager _navigationManager = navigationManager;

        [SupplyParameterFromForm]
        private UserWithRoleIdsUiObj Edit { get; set; } = default!;

        [Parameter]
        public Guid Id { get; set; }

        [Parameter]
        public Guid SelectedTenantId { get; set; }

        private bool _isLoading = false;
        private bool _isSaving = false;

        private readonly PaginationState _pagination = new() { ItemsPerPage = 10 };
        private string _searchValue = string.Empty;
        
        private IQueryable<RoleLightUiObj> _roleUiObjs = default!;

        private IQueryable<RoleLightUiObj>? FilteredRoles
        {
            get
            {
                return string.IsNullOrWhiteSpace(_searchValue)
                ? _roleUiObjs
                    : (_roleUiObjs.Where(a => a.Code.Contains(_searchValue, StringComparison.OrdinalIgnoreCase)));
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Edit = new UserWithRoleIdsUiObj()
            {
                Email = string.Empty,
                Firstname = string.Empty,
                Lastname = string.Empty
            };

            if (RendererInfo.IsInteractive)
            {
                await LoadOrRefreshDataAsync();
            }
        }

        private async Task LoadOrRefreshDataAsync()
        {
            _isLoading = true;
            var tasks = new List<Task>
                {
                    LoadUserAsync(),
                    LoadTenantAvailableRolesAsync(SelectedTenantId)
                };

            await Task.WhenAll(tasks);
            await SetSelectedRolesAsync();

            _isLoading = false;
        }

        private async Task LoadUserAsync()
        {
            var response = await _securityClient.GetTenantUserWithRolesIdsAsync(SelectedTenantId,Id);
            var result = await _responseManager.ManageAsync<UserWithTenantRoleIdsResult>(
                response,
                "Cannot retrieve data.",
                "An error occurred while loading user info.");
            if (result != null)
                Edit = result.ToUiObj();
        }

        private async Task LoadTenantAvailableRolesAsync(Guid tenantId)
        {
            var response = await _securityClient.GetTenantAvailableRolesAsync(tenantId);
            var result = await _responseManager.ManageAsync<List<RoleLightResult>>(
                response,
                "Cannot retrieve data.",
                "An error occurred while loading the roles.");

            if (result != null)
                _roleUiObjs = result.ToUiObjs().ToList().AsQueryable();
        }

        private async Task SetSelectedRolesAsync()
        {
            if (Edit.RoleIds != null && _roleUiObjs != null)
            {
                var linkedRoleIdsSet = new HashSet<Guid>(Edit.RoleIds);
                foreach (var role in _roleUiObjs)
                {
                    role.Selected = linkedRoleIdsSet.Contains(role.Id);
                }
            }

            await Task.CompletedTask;
        }

        private async Task HandleSubmitAsync()
        {
            _isSaving = true;
            var command = UserMappers.MapToUpdateUserWithTenantRoleIdsCommand([.. _roleUiObjs.Where(x => x.Selected).Select(x=>x.Id)]);

            var response = await _securityClient.UpdateTenantUserRolesAsync(SelectedTenantId, Edit.Id, command);
            var result = await _responseManager.ManageWithSuccessMsgAsync<bool>(
                response,
                "Save success.",
                "User roles updated.",
                null,
                "Cannot saving data.",
                "An error occurred while saving user roles.");

            if (result)
                _navigationManager.NavigateTo($"/tenant");

            _isSaving = false;
        }

        private void Close()
        {
            _navigationManager.NavigateTo($"/tenant");
        }

    }
}
