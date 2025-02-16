using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Collections.Frozen;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Roles.Commands;
using UbikLink.Security.Contracts.Roles.Results;
using UbikLink.Security.UI.Shared.Httpclients;
using UbikLink.Security.UI.Shared.State;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace UbikLink.Security.UI.Client.Components.Roles
{
    public partial class Roles(
        NavigationManager navigationManager,
        IHttpSecurityClient securityClient,
        RolesState state,
        IDialogService dialogService,
        ResponseManagerService responseManager)
    {
        private readonly NavigationManager _navigationManager = navigationManager;
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly RolesState _state = state;
        private readonly IDialogService _dialogService = dialogService;
        private readonly ResponseManagerService _responseManager = responseManager;

        [SupplyParameterFromQuery(Name = "state")]
        public string? UseState { get; set; }

        private FluentDataGrid<RoleUiObj> _grid = default!;
        private SelectColumn<RoleUiObj> _selectColumn = default!;

        private IQueryable<RoleUiObj>? _roles;
        private FrozenDictionary<Guid, AuthorizationStandardResult> _authorizationsDic = default!;

        private string _searchValue = string.Empty;
        private bool _isLoading = true;
        private bool _isDeletingFromToolbar = false;
        private readonly PaginationState _pagination = new() { ItemsPerPage = 20 };
        private int CountSelected
        {
            get
            {
                return FilteredRoles?.Count(a => a.Selected) ?? 0;
            }
        }

        private IQueryable<RoleUiObj>? FilteredRoles
        {
            get
            {
                return string.IsNullOrWhiteSpace(_searchValue)
                    ? _roles
                    : (_roles?.Where(a => a.Code.Contains(_searchValue, StringComparison.OrdinalIgnoreCase)
                        || a.Id.ToString().Contains(_searchValue, StringComparison.OrdinalIgnoreCase)));
            }
        }

        protected override async Task OnInitializedAsync()
        {
            if (RendererInfo.IsInteractive)
            {
                _isLoading = true;

                await LoadAuthorizationsAsync();
                await RefreshDataAsync(false);
                await ManageStateAsync();

                _isLoading = false;
            }
        }

        private async Task RefreshDataAsync(bool withLoadingStatus)
        {
            if (withLoadingStatus) _isLoading = true;

            var response = await _securityClient.GetAllRolesForAdminAsync();

            var result = await _responseManager.ManageAsync<IEnumerable<RoleAdminResult>>(
                response,
                "Cannot retrieve data.",
                "An error occurred while loading roles.");

            if (result != null)
            {
                _roles = result.MapToRoleUiObjs().ToList().AsQueryable();
            }

            if (withLoadingStatus) _isLoading = false;
        }

        private async Task LoadAuthorizationsAsync()
        {
            var response = await _securityClient.GetAllAuthorizationsForAdminAsync();

            var result = await _responseManager.ManageAsync<IEnumerable<AuthorizationStandardResult>>(
                response,
                "Cannot retrieve data.",
                "An error occurred while loading authorizations.");

            if (result != null)
            {
                _authorizationsDic = result.ToFrozenDictionary(x => x.Id, x => x);
            }
        }

        private async Task ManageStateAsync()
        {
            if (!string.IsNullOrEmpty(UseState) && bool.TryParse(UseState, out _))
            {
                await InvokeAsync(StateHasChanged);

                if (!(await ManageSelectedItemInState()))
                {
                    await ManagePageNumberAsync();
                    await ManageSortAsync();
                    ManageSearchValue();
                    ManageItemsPerPage();
                }
            }
            else
            {
                _state.CleanGridState();
            }
        }

        private async Task ManageSortAsync()
        {
            if (!string.IsNullOrEmpty(_state.SortColumnName) && _grid != null)
            {
                await _grid.SortByColumnAsync(_state.SortColumnName, _state.SortColumnDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending);
            }
        }

        private async Task ManagePageNumberAsync()
        {
            if (_pagination.CurrentPageIndex != _state.CurrentPageNumber)
                await _pagination.SetCurrentPageIndexAsync(_state.CurrentPageNumber < 1 ? 0 : _state.CurrentPageNumber);
        }

        private void ManageSearchValue()
        {
            if (_state.SearchValue != null)
                _searchValue = _state.SearchValue;
        }

        private void ManageItemsPerPage()
        {
            _pagination.ItemsPerPage = _state.ItemsPerPage;
        }

        private async Task<bool> ManageSelectedItemInState()
        {
            if (_state.SelectedId != null)
            {
                var newItem = _roles?.FirstOrDefault(a => a.Id == _state.SelectedId);

                if (newItem != null && _roles != null)
                {
                    //Select the item
                    newItem.Selected = true;

                    // Navigate to the page containing the new item (code is the primary sort)
                    var pageIndex = _roles!
                        .OrderBy(x => x.Code)
                        .ToList()
                        .FindIndex(a => a.Id == newItem.Id) / _pagination.ItemsPerPage;

                    await _pagination.SetCurrentPageIndexAsync(pageIndex);

                    //Reset the state
                    ManageItemsPerPage();
                    _state.UpdateGridState(pageIndex, null, "Code", "asc", _pagination.ItemsPerPage);

                    return true;
                }
            }

            return false;
        }

        private void AddRole()
        {
            UpdateStateForTheGrid();
            _navigationManager.NavigateTo($"/roles/add");
        }

        private void EditRole(Guid? id = null)
        {
            if (id == null && _selectColumn.SelectedItems.Count() == 1)
            {
                id = _selectColumn.SelectedItems.First().Id;
            }

            if (id == null) return;

            UpdateStateForTheGrid();
            _navigationManager.NavigateTo($"/roles/{id}");
        }

        private void UpdateStateForTheGrid()
        {
            var columnSortedName = ReflectionTweak.GetPrivateFieldValue<ColumnBase<RoleUiObj>?>(_grid, "_sortByColumn")?.Title;
            var sortDirection = (_grid.SortByAscending ?? false) ? "asc" : "desc";

            _state.UpdateGridState(_pagination.CurrentPageIndex, _searchValue, columnSortedName, sortDirection, _pagination.ItemsPerPage);
        }

        private async Task DeleteRoleAsync(Guid id)
        {
            var conf = await ShowConfirmDeleteMessageAsync();

            if (!conf.Cancelled)
            {
                _isLoading = true;

                var response = await _securityClient.DeleteRoleForAdminAsync(id);

                await _responseManager.ManageWithSuccessMsgAsync(
                    response,
                    "Delete success.",
                    $"Role {id} deleted.",
                    "Cannot delete data.",
                    "An error occurred while deleting role.");

                await _selectColumn.ClearSelectionAsync();
                await RefreshDataAsync(true);

                _isLoading = false;
            }
        }

        private async Task DeleteBatchAsync()
        {
            _isDeletingFromToolbar = true;
            var count = _selectColumn.SelectedItems.Count();

            if (count == 1)
            {
                await DeleteRoleAsync(_selectColumn.SelectedItems.First().Id);
            }
            else
            {
                if (count > 1)
                {
                    var conf = await ShowConfirmDeleteMessageAsync();
                    if (!conf.Cancelled)
                    {
                        _isLoading = true;

                        var response = await _securityClient.BatchDeleteRolesForAdminAsync(new BatchDeleteRoleCommand()
                        {
                            RoleIds = _selectColumn.SelectedItems.Select(a => a.Id).ToList()
                        });

                        await _responseManager.ManageWithSuccessMsgAsync(
                            response,
                            "Delete success.",
                            $"Roles deleted.",
                            "Cannot delete data.",
                            "An error occurred while deleting roles.");

                        await _selectColumn.ClearSelectionAsync();
                        await RefreshDataAsync(true);

                        _isLoading = false;
                    }
                }
            }
            _isDeletingFromToolbar = false;
        }

        private async Task<DialogResult> ShowConfirmDeleteMessageAsync()
        {
            var dialog = await _dialogService.ShowMessageBoxAsync(new DialogParameters<MessageBoxContent>()
            {
                Content = new()
                {
                    Title = "Are you sure ?",
                    MarkupMessage = new MarkupString("Deleting role(s) can have security impacts."),
                    Icon = new Icons.Regular.Size24.Warning(),
                    IconColor = Color.Warning,
                },
                PrimaryAction = "Yes",
                SecondaryAction = "No",
                Width = "300px",
            });

            return await dialog.Result;
        }
    }
}
