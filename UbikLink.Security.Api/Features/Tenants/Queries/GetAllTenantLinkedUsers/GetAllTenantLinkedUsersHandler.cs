using LanguageExt;
using UbikLink.Common.Api;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Tenants.Services;
using UbikLink.Security.Api.Features.Tenants.Services.Poco;

namespace UbikLink.Security.Api.Features.Tenants.Queries.GetAllTenantLinkedUsers
{
    public class GetAllTenantLinkedUsersHandler(TenantQueryService query, ICurrentUser currentUser)
    {
        private readonly TenantQueryService _query = query;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Either<IFeatureError, IEnumerable<UserWithLinkedRoles>>> Handle(Guid id)
        {
            return _currentUser.TenantId != id
                ? new ResourceNotFoundError("Tenant", new Dictionary<string, string>()
                {
                    { "Id", id.ToString() }
                })
                : await _query.GetTenantByIdAsync(id)
                .BindAsync(_query.GetAllTenantUsersWithRolesAsync);
        }
    }
}
