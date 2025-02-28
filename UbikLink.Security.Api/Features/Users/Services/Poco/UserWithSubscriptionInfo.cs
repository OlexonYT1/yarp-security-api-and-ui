using UbikLink.Common.Db;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Roles.Results;

namespace UbikLink.Security.Api.Features.Users.Services.Poco
{
    public class UserWithSubscriptionInfo
    {
        public Guid Id { get; set; }
        public required string AuthId { get; set; }
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Email { get; set; }
        public bool IsMegaAdmin { get; set; } = false;
        public Guid? SelectedTenantId { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public bool IsSubOwnerOfTheSelectetdTenant { get; init; } = false;
        public bool IsActiveInSelectedSubscription { get; set; }
        public List<Guid> OwnerOfSubscriptionsIds { get; set; } = [];
        public List<AuthorizationModel> SelectedTenantAuthorizations { get; set; } = [];
        public List<RoleModel> SelectedTenantRoles { get; set; } = [];
        public Guid Version { get; set; }
    }
}
