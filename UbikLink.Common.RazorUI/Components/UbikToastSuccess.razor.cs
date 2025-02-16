using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace UbikLink.Common.RazorUI.Components
{
    public partial class UbikToastSuccess : IToastContentComponent<UbikToastSuccessData>
    {
        [Parameter]
        public UbikToastSuccessData Content { get; set; } = default!;
    }

    public class UbikToastSuccessData
    {
        public string InnerMsg { get; set; } = default!;
    }
}
