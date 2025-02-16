using UbikLink.Common.Db;

namespace UbikLink.Security.Api.Data.Models
{
    public class SubscriptionUserModel : IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public bool IsOwner { get; set; } = false;
        public bool IsActivated { get; set; } = true;
        public Guid UserId { get; set; }
        public Guid Version { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
