using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Common.RazorUI.Errors;

namespace UbikLink.Common.RazorUI.Components
{
    public partial class UbikToastError : IToastContentComponent<UbikToastErrorData>
    {
        [Parameter]
        public UbikToastErrorData Content { get; set; } = default!;

        private async Task ShowErrorDetailsAsync()
        {
            await Content.OnErrorDetails.InvokeAsync();
        }
    }

    public class UbikToastErrorData
    {
        public string InnerMsg { get; set; } = default!;
        
        public EventCallback OnErrorDetails { get; set; }
    }
}
