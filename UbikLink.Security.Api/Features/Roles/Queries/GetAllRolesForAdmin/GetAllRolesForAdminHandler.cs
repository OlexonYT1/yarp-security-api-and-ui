using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Authorizations.Services;
using UbikLink.Security.Api.Features.Roles.Services;
using UbikLink.Security.Api.Features.Roles.Services.Poco;

namespace UbikLink.Security.Api.Features.Roles.Queries.GetAllRolesForAdmin
{
    public class GetAllRolesForAdminHandler(RoleQueryService query)
    {
        private readonly RoleQueryService _query = query;
        public async Task<IEnumerable<RoleWithAuthorizationIds>> Handle()
        {
            return await _query.GetAllRolesForAdminAsync();
        }
    }
}
