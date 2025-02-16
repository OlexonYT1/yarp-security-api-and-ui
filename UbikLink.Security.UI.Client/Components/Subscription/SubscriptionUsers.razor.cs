using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Subscriptions.Commands;
using UbikLink.Security.Contracts.Tenants.Results;
using UbikLink.Security.Contracts.Users.Results;
using UbikLink.Security.UI.Shared.Httpclients;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace UbikLink.Security.UI.Client.Components.Subscription
{
    public partial class SubscriptionUsers(NavigationManager navigationManager,
        IHttpSecurityClient securityClient,
        ResponseManagerService responseManager)
    {
        private readonly NavigationManager _navigationManager = navigationManager;
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly ResponseManagerService _responseManager = responseManager;

        [Parameter]
        public bool IsMainLoading { get; set; } = false;

        [Parameter]
        public SubscriptionUiObj? Subscription { get; set; }

        private SelectColumn<UserUiObj> _selectColumn = default!;
        private string _searchValue = string.Empty;
        private readonly PaginationState _pagination = new() { ItemsPerPage = 10 };

        private bool CanActivate
        {
            get
            {
                return Subscription?.Users.Count(x => x.IsActivated) < Subscription?.MaxUsers;
            }
        }

        private IQueryable<UserUiObj>? FilteredUsers
        {
            get
            {
                return string.IsNullOrWhiteSpace(_searchValue)
                    ? Subscription?.Users
                    : (Subscription?.Users.Where(a => a.Email.Contains(_searchValue, StringComparison.OrdinalIgnoreCase)
                        || a.Id.ToString().Contains(_searchValue, StringComparison.OrdinalIgnoreCase)
                        || a.Firstname.ToString().Contains(_searchValue, StringComparison.OrdinalIgnoreCase)
                        || a.Lastname.ToString().Contains(_searchValue, StringComparison.OrdinalIgnoreCase)));
            }
        }

        private int CountSelected
        {
            get
            {
                return FilteredUsers?.Count(a => a.Selected) ?? 0;
            }
        }

        private string LabelActivateDisactivate
        {
            get
            {
                var active = FilteredUsers?.FirstOrDefault(a => a.Selected)?.IsActivated;

                return active == null
                    ? "Activate"
                    : active.Value
                        ? "Disactivate"
                        : "Activate";
            }
        }

        private Icon IconActivateDisactivate
        {
            get
            {
                var selectedActive = FilteredUsers?.FirstOrDefault(a => a.Selected)?.IsActivated;

                return selectedActive == null
                    ? new Icons.Regular.Size16.PlayCircle()
                    : selectedActive.Value
                        ? new Icons.Regular.Size16.PauseCircle()
                        : new Icons.Regular.Size16.PlayCircle();
            }
        }

        private async Task ToggleUserActivationAsync(UserUiObj? currentUser = null)
        {
            if (Subscription == null)
                return;

            if (currentUser == null)
            {
                var selected = Subscription.Users.Where(x => x.Selected).ToList();

                if (selected.Count != 1)
                    return;

                currentUser = selected.First();
            }

            var isActivated = currentUser.IsActivated;

            if (isActivated)
            {
                isActivated = false;
                await SaveNewUserActivationStatusAsync(isActivated, currentUser);
            }
            else
            {
                var alreadyActivated = Subscription.Users.Count(t => t.IsActivated);
                if (alreadyActivated < Subscription.MaxUsers)
                {
                    isActivated = true;
                    await SaveNewUserActivationStatusAsync(isActivated, currentUser);
                }
                return;
            }
        }

        private async Task SaveNewUserActivationStatusAsync(bool activated, UserUiObj actualUser)
        {
            var command = new UpdateSubscriptionLinkedUserCommand()
            {
                IsActivated = activated,
                Firstname = actualUser.Firstname,
                Lastname = actualUser.Lastname,
                IsSubscriptionOwner = actualUser.IsOwner,
                Version = actualUser.Version
            };

            var response = await _securityClient.UpdateUserInSelectedSubscriptionForSubOwnerAsync(actualUser.Id, command);
            var strActivated = activated ? "activated" : "disactivated";

            var result = await _responseManager.ManageWithSuccessMsgAsync<UserWithInfoSubOwnerResult>(
                response,
                "Save success.",
                $"Linked user {{x}} {strActivated}.",
                "Id",
                "Cannot saving data.",
                "An error occurred while saving user.");

            if (result != null)
            {
                actualUser.IsActivated = result.IsActivated;
                actualUser.Version = result.Version;
            }
        }

        private async Task EditUserAsync(Guid? userId = null)
        {
            if (Subscription == null)
                return;

            if (userId == null)
            {
                var selected = Subscription.Users.Where(x => x.Selected).ToList();

                if (selected.Count != 1)
                    return;

                userId = selected.First().Id;
            }

            _navigationManager.NavigateTo($"/subscription/users/{userId}");
            await Task.CompletedTask;
        }
    }
}
