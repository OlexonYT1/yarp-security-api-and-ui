using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;


namespace UbikLink.Common.RazorUI.Components
{
    public partial class UbikFluentToolbar
    {
        private FluentOverflow _overFlowCompo = default!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }
    }
}
