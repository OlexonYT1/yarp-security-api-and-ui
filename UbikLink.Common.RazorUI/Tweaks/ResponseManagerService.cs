using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Common.RazorUI.Components;
using UbikLink.Common.RazorUI.Config;
using UbikLink.Common.RazorUI.Errors;

namespace UbikLink.Common.RazorUI.Tweaks
{
    public class ResponseManagerService(IMessageService messageService,
        IToastService toastService,
        IDialogService dialogService)
    {
        public async Task<T?> ManageAsync<T>(HttpResponseMessage response,
            string errTitle,
            string errMsg)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            else
            {
                await ManageServerErrorAsync(response,
                    errTitle,
                    errMsg);

                return default;
            }
        }

        public async Task ManageAsync(HttpResponseMessage response,
            string errTitle,
            string errMsg)
        {
            if (!response.IsSuccessStatusCode)
                await ManageServerErrorAsync(response,
                    errTitle,
                    errMsg);
        }

        public async Task ManageWithSuccessMsgAsync(HttpResponseMessage response,
            string successTitle,
            string successMsg,
            string errTitle,
            string errMsg)
        {
            if (response.IsSuccessStatusCode)
            {
                await ManagerServerSuccessAsync(response,
                successTitle,
                successMsg,
                string.Empty);
            }
            else
            {
                await ManageServerErrorAsync(response,
                errTitle,
                errMsg);
            }
        }

        public async Task<T?> ManageWithSuccessMsgAsync<T>(HttpResponseMessage response,
            string successTitle,
            string successMsg,
            string? successPropertyInMsg,
            string errTitle,
            string errMsg)
        {
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<T>
                    ();

                var successKey = string.Empty;

                if (successPropertyInMsg != null)
                {
                    var successProperty = result?.GetType()?.GetProperty(successPropertyInMsg);
                    successKey = successProperty?.GetValue(result)?.ToString() ?? string.Empty;
                }

                await ManagerServerSuccessAsync(response,
                successTitle,
                successMsg,
                successKey);

                return result;
            }
            else
            {
                await ManageServerErrorAsync(response,
                errTitle,
                errMsg);

                return default;
            }
        }

        private async Task ManagerServerSuccessAsync(HttpResponseMessage response,
        string successTitle,
        string successMsg,
        string successKey)
        {
            successMsg = !string.IsNullOrEmpty(successKey)
            ? successMsg.Replace("{x}", successKey)
            : successMsg;

            toastService.ShowToast<UbikToastSuccess, UbikToastSuccessData>(new ToastParameters<UbikToastSuccessData>()
            {
                Intent = ToastIntent.Success,
                Title = successTitle,
                Timeout = GeneralConfig.MESSAGES_SUCCESS_TIMEOUT,
                Content = new UbikToastSuccessData()
                {
                    InnerMsg = successMsg
                }
            });

            await messageService.ShowMessageBarAsync(options =>
            {
                options.Title = successTitle;
                options.Body = successMsg;
                options.Intent = MessageIntent.Success;
                options.ClearAfterNavigation = false;
                options.Section = GeneralConfig.MESSAGES_POSITION_NOTIFICATION;
                options.Timeout = GeneralConfig.MESSAGES_SUCCESS_TIMEOUT_IN_NOTIF;
            });
        }

        private async Task ManageServerErrorAsync(HttpResponseMessage response,
        string errTitle,
        string errMsg)
        {
            var _currentError = new ClientProblemDetails();

            try
            {
                _currentError = await response.Content.ReadFromJsonAsync<ClientProblemDetails>
                    ()
                    ?? GenerateStandardError((int)response.StatusCode);

            }
            catch
            {
                _currentError = GenerateStandardError((int)response.StatusCode);
            }

            toastService.ShowToast<UbikToastError, UbikToastErrorData>(new ToastParameters<UbikToastErrorData>()
            {
                Intent = ToastIntent.Error,
                Title = errTitle,
                Timeout = GeneralConfig.MESSAGES_ERROR_TIMEOUT,
                Content = new UbikToastErrorData()
                {
                    InnerMsg = errMsg,
                    OnErrorDetails = EventCallback.Factory.Create(this, () => ShowErrorDetailsAsync(_currentError))
                }
            });

            await messageService.ShowMessageBarAsync(options =>
            {
                options.Title = errTitle;
                options.Body = errMsg;
                options.Intent = MessageIntent.Error;
                options.ClearAfterNavigation = false;
                options.Section = GeneralConfig.MESSAGES_POSITION_NOTIFICATION;
                options.Timeout = GeneralConfig.MESSAGES_ERROR_TIMEOUT_IN_NOTIF;
                options.PrimaryAction = new ActionButton<Message>
                {
                    Text = "Show",
                    OnClick = (e) => ShowErrorDetailsAsync(_currentError!)
                };
            });
        }

        private async Task ShowErrorDetailsAsync(ClientProblemDetails errorDetails)
        {
            DialogParameters parameters = new()
            {
                Title = $"Server error",
                PrimaryAction = null,
                SecondaryAction = "Close",
                Width = "500px",
                TrapFocus = false,
                Modal = true,
                PreventScroll = true,
            };

            _ = await dialogService.ShowDialogAsync<ErrorDetailsDialog>
                (errorDetails, parameters);
        }

        private ClientProblemDetails GenerateStandardError(int statusCode = 500)
        {
            return new()
            {
                Title = "Unamanaged Error",
                Status = statusCode,
                Detail = "An unmanaged error occurred. Please try again later.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Instance = "No info",
                RequestId = "No info",
                TraceId = "No info",
            };
        }
    }
}
