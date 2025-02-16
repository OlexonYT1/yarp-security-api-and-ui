using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Common.RazorUI.Components
{
    public partial class UbikFormBottomActions
    {
        [Parameter] 
        public bool IsSavingInProgress { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClose { get; set; }

        [Parameter]
        public string? FormId { get; set; }
    }
}
