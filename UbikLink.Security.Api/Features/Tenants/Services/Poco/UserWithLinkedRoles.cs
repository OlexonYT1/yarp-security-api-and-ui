using UbikLink.Common.Db;

namespace UbikLink.Security.Api.Features.Tenants.Services.Poco
{
    public class UserWithLinkedRoles
    {
        public Guid Id { get; set; }
        public string AuthId { get; set; } = default!;
        public string Firstname { get; set; } = default!;
        public string Lastname { get; set; } = default!;
        public string Email { get; set; } = default!;
        public List<Guid> LinkedRoleIds { get; set; } = [];
        public Guid Version { get; set; }
    }
}
