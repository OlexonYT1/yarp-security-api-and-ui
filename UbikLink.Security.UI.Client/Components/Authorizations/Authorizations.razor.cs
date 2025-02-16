using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.UI.Client.Components.Shared;
using UbikLink.Security.UI.Shared.Httpclients;
using UbikLink.Security.UI.Shared.State;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace UbikLink.Security.UI.Client.Components.Authorizations
{
    public partial class Authorizations(
        NavigationManager navigationManager,
        IHttpSecurityClient securityClient,
        AuthorizationsState state,
        IDialogService dialogService,
        ResponseManagerService responseManager)
    {
        private readonly NavigationManager _navigationManager = navigationManager;
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly AuthorizationsState _state = state;
        private readonly IDialogService _dialogService = dialogService;
        private readonly ResponseManagerService _responseManager = responseManager;

        [SupplyParameterFromQuery(Name = "state")]
        public string? UseState { get; set; }

        private FluentDataGrid<AuthorizationUiObj> _grid = default!;
        private SelectColumn<AuthorizationUiObj> _selectColumn = default!;

        private IQueryable<AuthorizationUiObj>? _authorizations;

        private string _searchValue = string.Empty;
        private bool _isLoading = true;
        private bool _isDeletingFromToolbar = false;
        private readonly PaginationState _pagination = new() { ItemsPerPage = 20 };

        private int CountSelected
        {
            get
            {
                return FilteredAuthorizations?.Count(a => a.Selected) ?? 0;
            }
        }

        private IQueryable<AuthorizationUiObj>? FilteredAuthorizations
        {
            get
            {
                return string.IsNullOrWhiteSpace(_searchValue)
                    ? _authorizations
                    : (_authorizations?.Where(a => a.Code.Contains(_searchValue, StringComparison.OrdinalIgnoreCase)
                        || a.Id.ToString().Contains(_searchValue, StringComparison.OrdinalIgnoreCase)));
            }
        }

        protected override async Task OnInitializedAsync()
        {
            if (RendererInfo.IsInteractive)
            {
                _isLoading = true;

                await RefreshDataAsync(false);
                await ManageStateAsync();

                _isLoading = false;
            }
        }

        private async Task RefreshDataAsync(bool withLoadingStatus)
        {
            if (withLoadingStatus) _isLoading = true;

            var response = await _securityClient.GetAllAuthorizationsForAdminAsync();
            var result = await _responseManager.ManageAsync<IEnumerable<AuthorizationStandardResult>>(
                response,
                "Cannot retrieve data.",
                "An error occurred while loading authorizations.");

            if(result != null)
            {
                _authorizations = result.MapToAuthorizationUiObjs().ToList().AsQueryable();
            }

            if (withLoadingStatus) _isLoading = false;
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
                var newItem = _authorizations?.FirstOrDefault(a => a.Id == _state.SelectedId);

                if (newItem != null && _authorizations != null)
                {
                    //Select the item
                    newItem.Selected = true;

                    // Navigate to the page containing the new item (code is the primary sort)
                    var pageIndex = _authorizations!
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

        private void AddAuthorization()
        {
            UpdateStateForTheGrid();
            _navigationManager.NavigateTo($"/authorizations/add");
        }

        private void EditAuthorization(Guid? id = null)
        {
            if (id == null && _selectColumn.SelectedItems.Count() == 1)
            {
                id = _selectColumn.SelectedItems.First().Id;
            }

            if (id == null) return;

            UpdateStateForTheGrid();
            _navigationManager.NavigateTo($"/authorizations/{id}");
        }

        private void UpdateStateForTheGrid()
        {
            var columnSortedName = ReflectionTweak.GetPrivateFieldValue<ColumnBase<AuthorizationUiObj>?>(_grid, "_sortByColumn")?.Title;
            var sortDirection = (_grid.SortByAscending ?? false) ? "asc" : "desc";

            _state.UpdateGridState(_pagination.CurrentPageIndex, _searchValue, columnSortedName, sortDirection, _pagination.ItemsPerPage);
        }

        private async Task DeleteAuthorizationAsync(Guid id)
        {
            var conf = await ShowConfirmDeleteMessageAsync();

            if (!conf.Cancelled)
            {
                _isLoading = true;

                var response = await _securityClient.DeleteAuthorizationForAdminAsync(id);
                
                await _responseManager.ManageWithSuccessMsgAsync(
                    response,
                    "Delete success.",
                    $"Authorization {id} deleted.",
                    "Cannot delete data.",
                    "An error occurred while deleting authorization.");

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
                await DeleteAuthorizationAsync(_selectColumn.SelectedItems.First().Id);
            }
            else
            {
                if (count > 1)
                {
                    var conf = await ShowConfirmDeleteMessageAsync();
                    if (!conf.Cancelled)
                    {
                        _isLoading = true;

                        var response = await _securityClient.BatchDeleteAuthorizationsForAdminAsync(new BatchDeleteAuthorizationCommand()
                        {
                            AuthorizationIds = _selectColumn.SelectedItems.Select(a => a.Id).ToList()
                        });

                        await _responseManager.ManageWithSuccessMsgAsync(
                            response,
                            "Delete success.",
                            $"Authorizations deleted.",
                            "Cannot delete data.",
                            "An error occurred while deleting authorizations.");

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
                    MarkupMessage = new MarkupString("Deleting authorization(s) can have security impacts."),
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
