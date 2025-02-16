using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Roles.Results;
using UbikLink.Security.Contracts.Users.Results;
using UbikLink.Security.UI.Client.Components.Authorizations;
using UbikLink.Security.UI.Client.Components.Shared;
using UbikLink.Security.UI.Shared.Httpclients;
using UbikLink.Security.UI.Shared.State;

namespace UbikLink.Security.UI.Client.Components.Roles
{
    public partial class AddRole(NavigationManager navigationManager,
        RolesState state,
        IHttpSecurityClient securityClient,
        ResponseManagerService responseManager)
    {
        private readonly NavigationManager _navigationManager = navigationManager;
        private readonly RolesState _state = state;
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly ResponseManagerService _responseManager = responseManager;

        [SupplyParameterFromForm]
        private RoleUiObj Add { get; set; } = default!;

        private bool _isLoading = false;


        private IQueryable<AuthorizationUiObj> _authorizations = default!;

        private bool _isSaving = false;

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
            Add = new RoleUiObj();

            _isLoading = true;

            await LoadAllAvailableAuthorizationsAsync();

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

        private async Task HandleValidSubmitAsync()
        {
            _isSaving = true;

            Add.AuthorizationIds = FilteredAuthorizations?.Where(x => x.Selected).Select(a => a.Id).ToList() ?? [];
            var command = Add.MapToAddRoleCommand();

            var response = await _securityClient.AddRoleForAdminAsync(command);
            var result = await _responseManager.ManageWithSuccessMsgAsync<RoleAdminResult>(
            response,
            "Save success.",
                "Role {x} added.",
                "Id",
                "Cannot saving data.",
                "An error occurred while saving role.");

            if (result != null)
            {
                _state.SetSelectedId(result?.Id);
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
