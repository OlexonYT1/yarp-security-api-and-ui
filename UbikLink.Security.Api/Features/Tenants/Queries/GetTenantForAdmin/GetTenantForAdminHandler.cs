using LanguageExt;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Tenants.Services;

namespace UbikLink.Security.Api.Features.Tenants.Queries.GetTenantForAdmin
{
    public class GetTenantForAdminHandler(TenantQueryService query)
    {
        private readonly TenantQueryService _query = query;

        public async Task<Either<IFeatureError, TenantModel>> Handle(Guid id)
        {
            return await _query.GetTenantByIdForAdminAsync(id);
        }
    }
}
