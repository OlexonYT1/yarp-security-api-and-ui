using LanguageExt;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Authorizations.Services;

namespace UbikLink.Security.Api.Features.Authorizations.Queries.GetAuthorizationForAdmin
{
    public class GetAuthorizationForAdminHandler(AuthorizationQueryService query)
    {
        private readonly AuthorizationQueryService _query = query;
        public async Task<Either<IFeatureError,AuthorizationModel>> Handle(Guid id)
        {
            return await _query.GetAuthorizationAsync(id);
        }
    }
}
