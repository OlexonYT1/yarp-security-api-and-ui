using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using UbikLink.Common.RazorUI.Config;

namespace UbikLink.Common.RazorUI.Components
{
    public partial class UbikNotificationCenterPanel(IMessageService messageService) : IDialogContentComponent<GlobalState>
    {
        [Parameter]
        public GlobalState Content { get; set; } = default!;

        private async Task ClearMsgs()
        {
            messageService.Clear(GeneralConfig.MESSAGES_POSITION_NOTIFICATION);
            await Task.CompletedTask;
        }
    }
}
