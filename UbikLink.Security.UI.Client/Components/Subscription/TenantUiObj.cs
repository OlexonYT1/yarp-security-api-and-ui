using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using UbikLink.Security.Contracts.Subscriptions.Commands;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.Security.UI.Client.Components.Subscription
{
    public class TenantUiObj
    {
        public Guid Id { get; set; }
        public required Guid SubscriptionId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Label too long (100 character limit).")]
        public required string Label { get; set; }
        public bool IsActivated { get; set; } = true;
        public int UsersCount { get; set; } = default!;
        public List<Guid> LinkedUserIds { get; set; } = [];
        public Guid Version { get; set; }
        public bool Selected { get; set; } = false;
    }

    public static class TenantMappers
    {
        public static TenantUiObj ToUiObj(this TenantSubOwnerResult tenant)
        {
            return new TenantUiObj
            {
                Id = tenant.Id,
                SubscriptionId = tenant.SubscriptionId,
                Label = tenant.Label,
                IsActivated = tenant.IsActivated,
                UsersCount = tenant.LinkedUserIds.Count,
                LinkedUserIds = tenant.LinkedUserIds,
                Version = tenant.Version,
            };
        }

        public static IEnumerable<TenantUiObj> ToUiObjs(this IEnumerable<TenantSubOwnerResult> tenants)
        {
            return tenants.Select(t => t.ToUiObj());
        }

        public static AddSubscriptionLinkedTenantCommand ToAddSubscriptionLinkedTenant(this TenantUiObj tenant, IEnumerable<TenantUserUiObj> users)
        {
            return new AddSubscriptionLinkedTenantCommand
            {
                Label = tenant.Label,
                SubscriptionId = tenant.SubscriptionId,
                Active = tenant.IsActivated,
                LinkedUsersIds = users.Select(u => u.Id).ToList()
            };
        }

        public static UpdateSubscriptionLinkedTenantCommand ToUpdateSubscriptionLinkedTenant(this TenantUiObj tenant, IEnumerable<TenantUserUiObj> users)
        {
            return new UpdateSubscriptionLinkedTenantCommand
            {
                Label = tenant.Label,
                SubscriptionId = tenant.SubscriptionId,
                Version = tenant.Version,
                Active = tenant.IsActivated,
                LinkedUsersIds = users.Select(u => u.Id).ToList()
            };
        }
    }
}
