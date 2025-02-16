using Microsoft.FluentUI.AspNetCore.Components;

namespace UbikLink.Common.RazorUI.Components
{
    public partial class UbikNotificationCenter : IDisposable
    {
        private IDialogReference? _dialog;
        private readonly IMessageService _messageService;
        private readonly IDialogService _dialogService;

        public UbikNotificationCenter(IDialogService dialogService, IMessageService messageService)
        {
            _dialogService = dialogService;
            _messageService = messageService;
        }

        private Color BadgeColor
        {
            get
            {
                if(_messageService.AllMessages.Any(m=>m.Intent!.Value == MessageIntent.Error))
                    return Color.Error;

                return Color.Success;
            }
        }


        protected override async Task OnInitializedAsync()
        {
            _messageService.OnMessageItemsUpdatedAsync += UpdateCount;
            await Task.CompletedTask;
        }

        private async Task UpdateCount()
        {
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenNotificationCenterAsync()
        {
            _dialog = await _dialogService.ShowPanelAsync<UbikNotificationCenterPanel>(new DialogParameters<GlobalState>()
            {
                Alignment = HorizontalAlignment.Right,
                Title = $"Notifications",
                PrimaryAction = null,
                SecondaryAction = null,
                ShowDismiss = true,
                Modal = true,
            });
            DialogResult result = await _dialog.Result;
            HandlePanel(result);
        }

        private static void HandlePanel(DialogResult result)
        {
            if (result.Cancelled)
            {
                return;
            }

            if (result.Data is not null)
            {
                return;
            }
        }

        public void Dispose()
        {
            _messageService.OnMessageItemsUpdatedAsync -= UpdateCount;
        }
    }
}
