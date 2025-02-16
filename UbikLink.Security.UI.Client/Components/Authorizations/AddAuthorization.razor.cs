using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Net.Http.Json;
using UbikLink.Common.RazorUI.Components;
using UbikLink.Common.RazorUI.Config;
using UbikLink.Common.RazorUI.Errors;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.UI.Client.Components.Shared;
using UbikLink.Security.UI.Shared.Httpclients;
using UbikLink.Security.UI.Shared.State;

namespace UbikLink.Security.UI.Client.Components.Authorizations
{
    public partial class AddAuthorization(NavigationManager navigationManager,
        AuthorizationsState state,
        IHttpSecurityClient securityClient,
        ResponseManagerService responseManager)
    {
        private readonly NavigationManager _navigationManager = navigationManager;
        private readonly AuthorizationsState _state = state;
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly ResponseManagerService _responseManager = responseManager;

        [SupplyParameterFromForm]
        private AuthorizationUiObj Add { get; set; } = default!;

        private bool _isLoading = false;

        protected override Task OnInitializedAsync()
        {
            Add = new AuthorizationUiObj();
            return Task.CompletedTask;
        }

        private async Task HandleValidSubmitAsync()
        {
            _isLoading = true;

            var command = Add.MapToAddAuthorizationCommand();

            var response = await _securityClient.AddAuthorizationForAdminAsync(command);
            var result = await _responseManager.ManageWithSuccessMsgAsync<AuthorizationStandardResult>(
                response,
                "Save success.",
                "Authorization {x} added.",
                "Id",
                "Cannot saving data.",
                "An error occurred while saving authorization.");

            if (result != null)
            {
                _state.SetSelectedId(result?.Id);
                _navigationManager.NavigateTo($"/authorizations?state=true");
            }

            _isLoading = false;
        }

        private void Close()
        {
            _navigationManager.NavigateTo($"/authorizations?state=true");
        }
    }
}
