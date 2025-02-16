using System.ComponentModel.DataAnnotations;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Subscriptions.Results;
using UbikLink.Security.UI.Client.Components.Authorizations;
using UbikLink.Security.UI.Client.Components.Subscription;

namespace UbikLink.Security.UI.Client.Components.Settings
{
    public class SettingsUiObj
    {
        [Required(ErrorMessage = "Subscription Id is required.")]
        public string? SelectedSubscriptionId { get; set; }

        [Required(ErrorMessage = "Tenant Id is required.")]
        public string? SelectedTenantId { get; set; }
    }

    public static class SubscriptionMappers
    {
        public static SubscriptionUiObj MapToSubscriptionUiObj(this SubscriptionOwnerResult result)
        {
            return new SubscriptionUiObj
            {
                Id = result.Id,
                Label = result.Label,
                PlanName = result.PlanName,
                MaxUsers = result.MaxUsers,
                MaxTenants = result.MaxTenants,
                Version = result.Version
            };
        }
    }
}
