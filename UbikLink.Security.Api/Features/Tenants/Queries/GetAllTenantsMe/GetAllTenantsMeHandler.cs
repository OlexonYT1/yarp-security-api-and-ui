using LanguageExt;
using UbikLink.Common.Api;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Subscriptions.Services;
using UbikLink.Security.Api.Features.Tenants.Services;

namespace UbikLink.Security.Api.Features.Tenants.Queries.GetAllTenantsMe
{
    public class GetAllTenantsMeHandler(TenantQueryService queryService, ICurrentUser currentUser)
    {
        private readonly TenantQueryService _queryService = queryService;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<IEnumerable<TenantModel>> Handle()
        {
            return await _queryService.GetAllTenantsForUserAsync(_currentUser.Id);
        }
    }
}
