using LanguageExt;
using UbikLink.Common.Api;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Tenants.Services;

namespace UbikLink.Security.Api.Features.Tenants.Queries.GetTenantRoles
{
    public class GetTenantRolesHandler(TenantQueryService query, ICurrentUser currentUser)
    {
        private readonly TenantQueryService _query = query;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Either<IFeatureError, List<RoleModel>>> Handle(Guid id)
        {
            //The simple user can only access the tenant associated with his current request (connection).
            if (_currentUser.TenantId != id)
                return new ResourceNotFoundError("Tenant", new Dictionary<string, string>()
                {
                    { "Id", id.ToString() }
                });

            return await _query.GetTenantByIdAsync(id)
                .BindAsync(_query.GetTenantAvailableRoles);
        }
    }
}
