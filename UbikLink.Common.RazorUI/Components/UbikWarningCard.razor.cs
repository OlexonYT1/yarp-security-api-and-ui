using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Common.RazorUI.Components
{
    public partial class UbikWarningCard
    {
        [Parameter]
        public string FirstMsg { get; set; } = "Warning!";
        [Parameter]
        public string SecondMsg { get; set; } = "This is a warning message!";
    }
}
