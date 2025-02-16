using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Reflection.Metadata.Ecma335;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Roles.Results;
using UbikLink.Security.Contracts.Tenants.Results;
using UbikLink.Security.Contracts.Users.Results;
using UbikLink.Security.UI.Client.Components.Subscription;
using UbikLink.Security.UI.Shared.Httpclients;

namespace UbikLink.Security.UI.Client.Components.Tenant
{
    public partial class TenantUsers(NavigationManager navigationManager,
        IHttpSecurityClient securityClient,
        ResponseManagerService responseManager)
    {
        private readonly NavigationManager _navigationManager = navigationManager;
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly ResponseManagerService _responseManager = responseManager;

        [Parameter]
        public bool IsMainLoading { get; set; } = false;

        [Parameter]
        public List<UserWithTenantRoleIdsResult>? Users { get; set; } = null;

        [Parameter]
        public Dictionary<Guid, RoleLightResult>? AvailableRoles { get; set; } = null;

        private IQueryable<UserWithRoleIdsUiObj>? _users;
       

        private bool CanEdit
        {
            get
            {
                return _users?.Count(x => x.Selected) == 1;
            }
        }

        private SelectColumn<UserWithRoleIdsUiObj> _selectColumn = default!;
        private string _searchValue = string.Empty;
        private readonly PaginationState _pagination = new() { ItemsPerPage = 10 };

        private IQueryable<UserWithRoleIdsUiObj>? FilteredUsers
        {
            get
            {
                return string.IsNullOrWhiteSpace(_searchValue)
                    ? _users
                    : (_users?.Where(a => a.Email.Contains(_searchValue, StringComparison.OrdinalIgnoreCase)
                        || a.Id.ToString().Contains(_searchValue, StringComparison.OrdinalIgnoreCase)
                        || a.Firstname.ToString().Contains(_searchValue, StringComparison.OrdinalIgnoreCase)
                        || a.Lastname.ToString().Contains(_searchValue, StringComparison.OrdinalIgnoreCase)));
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            _users = Users?.ToUiObjs().ToList().AsQueryable();
            await Task.CompletedTask;
        }

        private async Task EditUserAsync(Guid? id = null)
        {
            if(id != null)
            {
                _navigationManager.NavigateTo($"/tenant/users/{id}");
                await Task.CompletedTask;
                return;
            }

            var selected = _users?.Where(x => x.Selected).ToList();

            if (selected !=null && selected.Count == 1)
            {
                _navigationManager.NavigateTo($"/tenant/users/{selected.First().Id}");
                await Task.CompletedTask;
            }
        }
    }
}
