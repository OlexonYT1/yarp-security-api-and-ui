using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Common.RazorUI.Components
{
    public partial class UbikFluentBreadcrumb
    {
        [Parameter]
        public List<BreadcrumbListItem> Items { get; set; } = [];

        [Parameter]
        public bool WithClose { get; set; } = false;

        [Parameter]
        public string? CloseUrl { get; set; }
    }

    public class BreadcrumbListItem
    {
        public int Position { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Url { get; set; } = default!;
    }
}
