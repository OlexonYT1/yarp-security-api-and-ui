using UbikLink.Common.Db;

namespace UbikLink.Security.Api.Data.Models
{
    public class AuthorizationModel : IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public string? Description { get; set; }
        public Guid Version { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
