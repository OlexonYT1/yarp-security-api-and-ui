using UbikLink.Common.Db;

namespace UbikLink.Security.Api.Features.Roles.Services.Poco
{
    public class RoleWithAuthorizationIds
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public string? Description { get; set; }
        public List<Guid> AuthorizationIds { get; set; } = [];
        public Guid Version { get; set; }
    }
}
