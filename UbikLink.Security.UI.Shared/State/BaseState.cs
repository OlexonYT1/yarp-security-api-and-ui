namespace UbikLink.Security.UI.Shared.State
{
    public abstract class BaseState
    {
        public int CurrentPageNumber { get; private set; } = 0;
        public string? SortColumnName { get; private set; }
        public string SortColumnDirection { get; private set; } = "asc";
        public string? SearchValue { get; private set; }
        public int ItemsPerPage { get; private set; } = 20;
        public Guid? SelectedId { get; private set; }


        public void UpdateGridState(int pageIndex, string? searchValue, string? sortColumnName, string sortColumnDirection, int itemsPerPage)
        {
            CurrentPageNumber = pageIndex;
            SearchValue = searchValue;
            SelectedId = null;
            SortColumnName = sortColumnName;
            SortColumnDirection = sortColumnDirection;
            ItemsPerPage = itemsPerPage;
        }

        public void SetSelectedId(Guid? selectedId)
        {
            CurrentPageNumber = 1;
            SearchValue = null;
            SelectedId = selectedId;
            SortColumnName = "Code";
            SortColumnDirection = "asc";
        }

        public void CleanGridState()
        {
            CurrentPageNumber = 1;
            SearchValue = null;
            SelectedId = null;
            SortColumnName = null;
            SortColumnDirection = "asc";
            ItemsPerPage = 20;
        }
    }

    public class AuthorizationsState : BaseState
    {

    }

    public class RolesState : BaseState
    {

    }
}
