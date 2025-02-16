using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Roles.Results;
using UbikLink.Security.UI.Client.Components.Shared;
using UbikLink.Security.UI.Client.Components.Subscription;
using UbikLink.Security.UI.Shared.Httpclients;
using UbikLink.Security.UI.Shared.State;

namespace UbikLink.Security.UI.Client.Components.Roles
{
    public partial class UpdateRole(
        NavigationManager navigationManager,
        IHttpSecurityClient securityClient,
        RolesState state,
        ResponseManagerService responseManager)
    {
        private readonly NavigationManager _navigationManager = navigationManager;
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly RolesState _state = state;
        private readonly ResponseManagerService _responseManager = responseManager;

        [Parameter]
        public Guid Id { get; set; }

        [SupplyParameterFromForm]
        private RoleUiObj Edit { get; set; } = default!;

        private bool _isSaving = false;

        private bool _isLoading = false;

        private IQueryable<AuthorizationUiObj> _authorizations = default!;


        private readonly PaginationState _pagination = new() { ItemsPerPage = 10 };
        private string _searchValue = string.Empty;

        private IQueryable<AuthorizationUiObj>? FilteredAuthorizations
        {
            get
            {
                return string.IsNullOrWhiteSpace(_searchValue)
                    ? _authorizations
                    : (_authorizations.Where(a => a.Code.Contains(_searchValue, StringComparison.OrdinalIgnoreCase)
                        || a.Id.ToString().Contains(_searchValue, StringComparison.OrdinalIgnoreCase)));
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Edit = new RoleUiObj();

            if (RendererInfo.IsInteractive)
            {
                await LoadOrRefreshDataAsync();
            }
        }

        private async Task LoadOrRefreshDataAsync()
        {
            _isLoading = true;

            var loadTasks = new List<Task>
                {
                    LoadAllAvailableAuthorizationsAsync(),
                    LoadRoleAsync()
                };

            await Task.WhenAll(loadTasks);
            await SetSelectedAuthorizationsAsync();

            _isLoading = false;
        }

        private async Task LoadAllAvailableAuthorizationsAsync()
        {
            var response = await _securityClient.GetAllAuthorizationsForAdminAsync();
            var result = await _responseManager.ManageAsync<IEnumerable<AuthorizationStandardResult>>(
                    response,
                    "Cannot retrieve data.",
                    "An error occurred while loading authorizations.");

            if (result != null)
            {
                _authorizations = result.MapToAuthorizationUiObjs().ToList().AsQueryable();
            }
        }

        private async Task LoadRoleAsync()
        {
            var response = await _securityClient.GetRoleForAdminByIdAsync(Id);

            var result = await _responseManager.ManageAsync<RoleAdminResult>(
                response,
                "Cannot retrieve data.",
                "An error occurred while loading role.");

            if (result != null)
            {
                Edit = result.MapToRoleUiObj();
            }
        }

        private async Task SetSelectedAuthorizationsAsync()
        {
            if (Edit.AuthorizationIds != null && _authorizations != null)
            {
                var linkedAuthIdsSet = new HashSet<Guid>(Edit.AuthorizationIds);
                foreach (var auth in _authorizations)
                {
                    auth.Selected = linkedAuthIdsSet.Contains(auth.Id);
                }
            }

            await Task.CompletedTask;
        }

        private async Task HandleValidSubmitAsync()
        {
            _isSaving = true;
            Edit.AuthorizationIds = [.. _authorizations.Where(x => x.Selected).Select(x=>x.Id)];

            var command = Edit.MapToUpdateRoleCommand();

            var response = await _securityClient.UpdateRoleForAdminAsync(Id, command);

            var result = await _responseManager.ManageWithSuccessMsgAsync<RoleAdminResult>(
            response,
                "Save success.",
                "Role {x} updated.",
                "Id",
                $"Cannot saving data.",
                "An error occurred while saving role.");

            if (result != null)
            {
                _state.SetSelectedId(result!.Id);
                _navigationManager.NavigateTo($"/roles?state=true");
            }

            _isSaving = false;
        }

        private void Close()
        {
            _navigationManager.NavigateTo($"/roles?state=true");
        }
    }
}
