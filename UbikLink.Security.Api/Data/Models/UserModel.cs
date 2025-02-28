using UbikLink.Common.Db;

namespace UbikLink.Security.Api.Data.Models
{
    public class UserModel : IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required string AuthId { get; set; }
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Email { get; set; }
        public bool IsMegaAdmin { get; set; } = false;
        public bool IsEmailVerified { get; set; } = false;
        public string? ActivationCode { get; set; }
        public Guid? SelectedTenantId { get; set; }
        public Guid Version { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
