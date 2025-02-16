using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace UbikLink.Common.RazorUI.Components
{
    public partial class UbikGridSearchAndItemsPerPage
    {
        [Parameter]
        public string? SearchValue { get; set; }

        private string? BoundSearchValue
        {
            get => SearchValue;
            set
            {
                if (SearchValue == value)
                    return;

                SearchValue = value;
                SearchValueChanged.InvokeAsync(value);
            }
        }

        [Parameter]
        public EventCallback<string> SearchValueChanged { get; set; }

        [Parameter]
        public EventCallback<int> SelectedItemsPerPageChanged { get; set; }

        [Parameter]
        public int SelectedItemsPerPage { get; set; }

        [Parameter]
        public string PlaceHolder { get; set; } = string.Empty;

        private int BoundSelectedItemsPerPage
        {
            get => SelectedItemsPerPage;
            set
            {
                if (SelectedItemsPerPage == value)
                    return;

                SelectedItemsPerPage = value;
                SelectedItemsPerPageChanged.InvokeAsync(value);
            }
        }

        private static readonly List<int> _itemsPerPages = [10, 15, 20, 25, 50];
    }
}
