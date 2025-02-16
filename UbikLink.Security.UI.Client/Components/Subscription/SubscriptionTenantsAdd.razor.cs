using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Diagnostics;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Tenants.Results;
using UbikLink.Security.Contracts.Users.Results;
using UbikLink.Security.UI.Client.Components.Authorizations;
using UbikLink.Security.UI.Shared.Httpclients;

namespace UbikLink.Security.UI.Client.Components.Subscription
{
    public partial class SubscriptionTenantsAdd(IHttpSecurityClient securityClient,
        ResponseManagerService responseManager,
        NavigationManager navigationManager)
    {
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly ResponseManagerService _responseManager = responseManager;
        private readonly NavigationManager _navigationManager = navigationManager;

        [SupplyParameterFromForm]
        private TenantUiObj Add { get; set; } = default!;

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
            _isLoading = true;

            Add = new TenantUiObj()
            {
                Label = string.Empty,
                SubscriptionId = SelectedSubscriptionId
            };

            if (RendererInfo.IsInteractive)
            {
                await LoadAllAvailableUsersAsync();
            }

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

        private async Task HandleSubmitAsync()
        {
            _isSaving = true;
            var command = Add.ToAddSubscriptionLinkedTenant(_userUiObjs.Where(x => x.Selected));

            var response = await _securityClient.AddTenantInSelectedSubscriptionForSubOwnerAsync(command);
            var result = await _responseManager.ManageWithSuccessMsgAsync<UserLinkedTenantSubOwnerResult>(
                response,
                "Save success.",
                "Linked tenant {x} added.",
                "Id",
                "Cannot saving data.",
                "An error occurred while saving authorization.");

            if (result != null)
                _navigationManager.NavigateTo($"/subscription");

            _isLoading = false;
        }

        private void Close()
        {
            _navigationManager.NavigateTo($"/subscription");
        }
    }
}
