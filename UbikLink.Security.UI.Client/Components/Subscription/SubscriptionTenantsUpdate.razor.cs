using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Buffers;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Tenants.Results;
using UbikLink.Security.Contracts.Users.Results;
using UbikLink.Security.UI.Shared.Httpclients;

namespace UbikLink.Security.UI.Client.Components.Subscription
{
    public partial class SubscriptionTenantsUpdate(IHttpSecurityClient securityClient,
        ResponseManagerService responseManager,
        NavigationManager navigationManager)
    {
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly ResponseManagerService _responseManager = responseManager;
        private readonly NavigationManager _navigationManager = navigationManager;

        [SupplyParameterFromForm]
        private TenantUiObj Edit { get; set; } = default!;

        [Parameter]
        public Guid Id { get; set; }

        [Parameter]
        public Guid SelectedSubscriptionId { get; set; } = default!;

        private IQueryable<TenantUserUiObj> _userUiObjs = default!;

        private bool _isLoading = false;
        private bool _isSaving = false;

        private string? activeId = "tab-info";

        private readonly PaginationState _pagination = new() { ItemsPerPage = 10 };
        private string _searchValue = string.Empty;

        private IQueryable<TenantUserUiObj>? FilteredUsers
        {
            get
            {
                return string.IsNullOrWhiteSpace(_searchValue)
                ? _userUiObjs
                    : (_userUiObjs.Where(a => a.Email.Contains(_searchValue, StringComparison.OrdinalIgnoreCase)
                        || a.Id.ToString().Contains(_searchValue, StringComparison.OrdinalIgnoreCase)
                        || a.Firstname.ToString().Contains(_searchValue, StringComparison.OrdinalIgnoreCase)
                        || a.Lastname.ToString().Contains(_searchValue, StringComparison.OrdinalIgnoreCase)));
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Edit = new TenantUiObj()
            {
                Label = string.Empty,
                SubscriptionId = SelectedSubscriptionId
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
                    LoadTenantAsync(),
                    LoadAllAvailableUsersAsync()
                };

            await Task.WhenAll(tasks);
            await SetSelectedUsersAsync();

            _isLoading = false;
        }

        private async Task LoadAllAvailableUsersAsync()
        {
            var response = await _securityClient.GetSelectedSubscriptionLinkedUsersForSubOwnerAsync();
            var result = await _responseManager.ManageAsync<IEnumerable<UserSubOwnerResult>>(
                    response,
                    "Cannot retrieve data.",
                    "An error occurred while loading users.");

            if (result != null)
            {
                var tmp = result.ToTenantUserUiObjs(null).ToList();
                _userUiObjs = tmp.AsQueryable();
            }
        }

        private async Task LoadTenantAsync()
        {
            var response = await _securityClient.GetSubscriptionLinkedTenantForSubOwnerAsync(Id);
            var result = await _responseManager.ManageAsync<TenantSubOwnerResult>(
                    response,
                    "Cannot retrieve data.",
                    "An error occurred while loading then tenant.");

            if (result != null)
            {
                Edit = result.ToUiObj();
            };
        }

        private async Task SetSelectedUsersAsync()
        {
            if (Edit.LinkedUserIds != null && _userUiObjs != null)
            {
                var linkedUserIdsSet = new HashSet<Guid>(Edit.LinkedUserIds);
                foreach (var user in _userUiObjs)
                {
                    user.Selected = linkedUserIdsSet.Contains(user.Id);
                }
            }

            await Task.CompletedTask;
        }

        private async Task HandleSubmitAsync()
        {
            _isSaving = true;
            var command = Edit.ToUpdateSubscriptionLinkedTenant(_userUiObjs.Where(x => x.Selected));

            var response = await _securityClient.UpdateTenantInSelectedSubscriptionForSubOwnerAsync(Edit.Id,command);
            var result = await _responseManager.ManageWithSuccessMsgAsync<UserLinkedTenantSubOwnerResult>(
                response,
                "Save success.",
                "Linked tenant {x} updated.",
                "Id",
                "Cannot saving data.",
                "An error occurred while saving tenant.");

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
