using UbikLink.Common.Db;

namespace UbikLink.Security.Api.Data.Models
{
    public class TenantUserModel : IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public Guid Version { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
