using LanguageExt;
using UbikLink.Common.Api;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Features.Tenants.Services.Poco;
using UbikLink.Security.Api.Features.Tenants.Services;

namespace UbikLink.Security.Api.Features.Tenants.Queries.GetTenantLinkedUser
{
    public class GetTenantLinkedUserHandler(TenantQueryService query, ICurrentUser currentUser)
    {
        private readonly TenantQueryService _query = query;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Either<IFeatureError, UserWithLinkedRoles>> Handle(Guid tenantId, Guid userId)
        {
            return _currentUser.TenantId != tenantId
                ? new ResourceNotFoundError("Tenant", new Dictionary<string, string>()
                {
                    { "Id", tenantId.ToString() }
                })
                : await _query.GetTenantByIdAsync(tenantId)
                .BindAsync(t=>_query.GetTenantUserWithRolesAsync(t,userId));
        }
    }
}
