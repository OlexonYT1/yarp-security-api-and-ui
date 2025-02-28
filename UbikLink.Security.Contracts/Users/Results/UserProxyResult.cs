using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Roles.Results;

namespace UbikLink.Security.Contracts.Users.Results
{
    public record UserProxyResult
    {
        public Guid Id { get; init; }
        public required string AuthId { get; init; }
        public required string Firstname { get; init; }
        public required string Lastname { get; init; }
        public required string Email { get; init; }
        public bool IsActivatedInSelectedSubscription { get; init; } = true;
        public bool IsEmailVerified { get; init; } = false;
        public bool IsMegaAdmin { get; init; } = false;
        public IEnumerable<Guid> OwnerOfSubscriptionsIds { get; init; } = default!;
        public Guid? SelectedTenantId { get; init; }
        public bool IsSubOwnerOfTheSelectedTenant { get; init; } = false;
        public List<AuthorizationLightResult> SelectedTenantAuthorizations { get; init; } = [];
        public  List<RoleLightResult> SelectedTenantRoles { get; init; } = [];
        public Guid Version { get; init; }
    }
}
