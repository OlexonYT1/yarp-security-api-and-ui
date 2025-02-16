using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.Contracts.Subscriptions.Commands;
using UbikLink.Security.Contracts.Tenants.Results;
using UbikLink.Security.Contracts.Users.Results;
using UbikLink.Security.UI.Shared.Httpclients;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace UbikLink.Security.UI.Client.Components.Subscription
{
    public partial class SubscriptionTenants(NavigationManager navigationManager,
        IHttpSecurityClient securityClient,
        ResponseManagerService responseManager,
        IDialogService dialogService)
    {
        private readonly NavigationManager _navigationManager = navigationManager;
        private readonly IHttpSecurityClient _securityClient = securityClient;
        private readonly ResponseManagerService _responseManager = responseManager;
        private readonly IDialogService _dialogService = dialogService;

        [Parameter]
        public bool IsMainLoading { get; set; } = false;

        [Parameter]
        public SubscriptionUiObj? Subscription { get; set; }

        [Parameter]
        public UserMeResult? UserInfo { get; set; }

        [Parameter]
        public EventCallback OnTenantDeleted { get; set; }

        private string LabelActivateDisactivate
        {
            get
            {
                var selectedActive = Subscription?.Tenants?.FirstOrDefault(a => a.Selected)?.IsActivated;

                return selectedActive == null
                    ? "Activate"
                    : selectedActive.Value
                        ? "Disactivate"
                        : "Activate";
            }
        }

        private Icon IconActivateDisactivate
        {
            get
            {
                var selectedActive = Subscription?.Tenants?.FirstOrDefault(a => a.Selected)?.IsActivated;

                return selectedActive == null
                    ? new Icons.Regular.Size16.PlayCircle()
                    : selectedActive.Value
                        ? new Icons.Regular.Size16.PauseCircle()
                        : new Icons.Regular.Size16.PlayCircle();
            }
        }

        private async Task OnSelectTenantAsync(bool selected, Guid selectedId)
        {
            if (Subscription != null)
            {
                var oldSelected = Subscription.Tenants.Where(t => t.Selected).FirstOrDefault();
                if (oldSelected != null)
                    oldSelected.Selected = false;

                if (selected)
                {
                    var newSelected = Subscription.Tenants.Where(t => t.Id == selectedId).First();
                    newSelected.Selected = true;
                }
            }

            await Task.CompletedTask;
        }

        private async Task AddTenantAsync()
        {
            _navigationManager.NavigateTo($"/subscription/tenants/add");
            await Task.CompletedTask;
        }

        private async Task EditTenantAsync()
        {
            if (Subscription == null)
                return;

            var selected = Subscription.Tenants.Where(x => x.Selected).ToList();

            if (selected.Count == 1)
            {
                _navigationManager.NavigateTo($"/subscription/tenants/{selected.First().Id}");
                await Task.CompletedTask;
            }
        }

        private async Task ToggleTenantActivationAsync()
        {
            if (Subscription == null)
                return;

            var selected = Subscription.Tenants.Where(x => x.Selected).ToList();

            if (selected.Count != 1)
                return;

            var tenant = selected.First();
            var isActivated = tenant.IsActivated;

            if (isActivated)
            {
                isActivated = false;
                await SaveNewTenantActivationStatusAsync(isActivated, tenant);
            }
            else
            {
                var alreadyActivated = Subscription.Tenants.Count(t => t.IsActivated);
                if (alreadyActivated < Subscription.MaxTenants)
                {
                    isActivated = true;
                    await SaveNewTenantActivationStatusAsync(isActivated, tenant);
                }
                return;
            }
        }

        private async Task SaveNewTenantActivationStatusAsync(bool activated, TenantUiObj actualTenant)
        {
            var command = new UpdateSubscriptionLinkedTenantCommand()
            {
                Active = activated,
                Label = actualTenant.Label,
                LinkedUsersIds = actualTenant.LinkedUserIds,
                SubscriptionId = actualTenant.SubscriptionId,
                Version = actualTenant.Version
            };

            var response = await _securityClient.UpdateTenantInSelectedSubscriptionForSubOwnerAsync(actualTenant.Id, command);
            var strActivated = activated ? "activated" : "disactivated";

            var result = await _responseManager.ManageWithSuccessMsgAsync<TenantSubOwnerResult>(
                response,
                "Save success.",
                $"Linked tenant {{x}} {strActivated}.",
                "Id",
                "Cannot saving data.",
                "An error occurred while saving tenant.");

            if (result != null)
            {
                actualTenant.IsActivated = result.IsActivated;
                actualTenant.Version = result.Version;
            }
        }

        private async Task DeleteTenantAsync()
        {
            if (Subscription == null)
                return;

            var selected = Subscription.Tenants.Where(x => x.Selected).ToList();

            if (selected.Count != 1)
                return;
            
            var tenant = selected.First();

            var conf = await ShowConfirmDeleteTenantMessageAsync();

            if (!conf.Cancelled)
            {
                var response = await _securityClient.DeleteSubscriptionLinkedTenantForSubOwnerAsync(tenant.Id);

                await _responseManager.ManageWithSuccessMsgAsync(
                    response,
                    "Delete success.",
                    $"Tenant {tenant.Id} deleted.",
                    "Cannot delete data.",
                    "An error occurred while deleting tenant.");

                await OnTenantDeleted.InvokeAsync();
            }
        }

        private async Task<DialogResult> ShowConfirmDeleteTenantMessageAsync()
        {
            var dialog = await _dialogService.ShowMessageBoxAsync(new DialogParameters<MessageBoxContent>()
            {
                Content = new()
                {
                    Title = "Are you sure ?",
                    MarkupMessage = new MarkupString("Deleting tenant can have security impacts."),
                    Icon = new Icons.Regular.Size24.Warning(),
                    IconColor = Color.Warning,
                },
                PrimaryAction = "Yes",
                SecondaryAction = "No",
                Width = "300px",
            });

            return await dialog.Result;
        }
    }
}
