using UbikLink.Common.Db;

namespace UbikLink.Security.Api.Data.Models
{
    public class RoleAuthorizationModel : IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid AuthorizationId { get; set; }
        public Guid Version { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
