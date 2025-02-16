using LanguageExt;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Authorizations.Services;
using UbikLink.Security.Api.Features.Roles.Services;
using UbikLink.Security.Api.Features.Roles.Services.Poco;

namespace UbikLink.Security.Api.Features.Roles.Queries.GetRoleForAdmin
{
    public class GetRoleForAdminHandler(RoleQueryService query)
    {
        private readonly RoleQueryService _query = query;
        public async Task<Either<IFeatureError, RoleWithAuthorizationIds>> Handle(Guid id)
        {
            return await _query.GetRoleForAdminAsync(id);
        }
    }
}
