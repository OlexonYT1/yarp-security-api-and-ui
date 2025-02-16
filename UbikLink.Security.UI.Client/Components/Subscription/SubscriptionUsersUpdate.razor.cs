using Microsoft.AspNetCore.Components;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Tenants.Results;
using UbikLink.Security.Contracts.Users.Results;
using UbikLink.Security.UI.Shared.Httpclients;

namespace UbikLink.Security.UI.Client.Components.Subscription
{
    public partial class SubscriptionUsersUpdate(IHttpSecurityClient securityClient,
        ResponseManagerService responseManager,
        NavigationManager navigationManager)
    {
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly ResponseManagerService _responseManager = responseManager;
        private readonly NavigationManager _navigationManager = navigationManager;

        [SupplyParameterFromForm]
        private UserUiObj Edit { get; set; } = default!;

        [Parameter]
        public Guid Id { get; set; }

        [Parameter]
        public Guid SelectedSubscriptionId { get; set; } = default!;

        private bool _isSaving = false;


        protected override async Task OnInitializedAsync()
        {
            Edit = new UserUiObj()
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
            await LoadUserAsync();
        }

        private async Task LoadUserAsync()
        {
            var response = await _securityClient.GetSubscriptionLinkedUserForSubOwnerAsync(Id);
            var result = await _responseManager.ManageAsync<UserWithInfoSubOwnerResult>(
                    response,
                    "Cannot retrieve data.",
                    "An error occurred while loading the user.");

            if (result != null)
            {
                Edit = result.ToUserUiObj();
            };
        }

        private async Task HandleSubmitAsync()
        {
            _isSaving = true;

            var command = Edit.ToUpdateSubscriptionLinkedUserCommand();

            var response = await _securityClient.UpdateUserInSelectedSubscriptionForSubOwnerAsync(Edit.Id, command);
            var result = await _responseManager.ManageWithSuccessMsgAsync<UserWithInfoSubOwnerResult>(
                response,
                "Save success.",
                "Linked user {x} updated.",
                "Id",
                "Cannot saving data.",
                "An error occurred while saving user.");

            if (result != null)
                _navigationManager.NavigateTo($"/subscription");

            _isSaving = false;
        }

        private void Close()
        {
            _navigationManager.NavigateTo($"/subscription");
        }
    }
}
