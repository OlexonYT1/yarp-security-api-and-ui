using UbikLink.Common.Db;

namespace UbikLink.Security.Api.Data.Models
{
    public class TenantModel : IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public required string Label { get; set; }
        public bool IsActivated { get; set; } = true;
        public Guid Version { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
