using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace UbikLink.Common.RazorUI.Components
{
    public partial class UbikSessionManager(IDialogService dialogService,
        IJSRuntime js,
        NavigationManager navigationManager) : IDisposable
    {
        private ElementReference? BtnLogout { get; set; } = default!;

        private readonly IDialogService _dialog = dialogService;
        private readonly IJSRuntime _js = js;
        private readonly NavigationManager _navigationManager = navigationManager;

        private DotNetObjectReference<UbikSessionManager>? _dotNetHelper;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _dotNetHelper = DotNetObjectReference.Create(this);
                await _js.InvokeVoidAsync("Helpers.setDotNetHelper", _dotNetHelper);
            }
        }

        [JSInvokable]
        public async Task OnSessionTimeout()
        {
            var dialog = await _dialog.ShowMessageBoxAsync(new DialogParameters<MessageBoxContent>()
            {
                Content = new()
                {
                    Title = "Session will expire soon",
                    MarkupMessage = new MarkupString("Do you want to stay logged ?"),
                    Icon = new Icons.Regular.Size48.Timer(),
                    IconColor = Color.Info,
                },
                PrimaryAction = "Yes",
                SecondaryAction = "No",
                Width = "300px",
            });

            var result = await dialog.Result;

            if (result.Cancelled && BtnLogout != null)
            {
                await _js.InvokeVoidAsync("Helpers.triggerClick", BtnLogout);
            }
            else
            {
                _navigationManager.NavigateTo(_navigationManager.Uri, true);
            }
        }

        public void Dispose()
        {
            _dotNetHelper?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
