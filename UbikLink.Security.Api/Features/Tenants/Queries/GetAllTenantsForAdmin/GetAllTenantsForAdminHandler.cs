using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Tenants.Services;

namespace UbikLink.Security.Api.Features.Tenants.Queries.GetAllTenantsForAdmin
{
    public class GetAllTenantsForAdminHandler(TenantQueryService query)
    {
        private readonly TenantQueryService _query = query;
        public async Task<IEnumerable<TenantModel>> Handle()
        {
            return await _query.GetAllTenantsAsync();
        }
    }
}
