using UbikLink.Common.Db;

namespace UbikLink.Security.Api.Data.Models
{
    public class RoleModel : IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public string? Description { get; set; }
        public Guid? TenantId { get; set; } // A role can be a general role and a specific role for a tenant
        public Guid Version { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
