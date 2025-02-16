using LanguageExt;
using UbikLink.Common.Api;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Features.Subscriptions.Services.Poco;
using UbikLink.Security.Api.Features.Subscriptions.Services;
using UbikLink.Security.Contracts.Subscriptions.Commands;

namespace UbikLink.Security.Api.Features.Subscriptions.Queries.GetSubscriptionLinkedTenantForOwner
{
    public class GetSubscriptionLinkedTenantForOwnerHandler(SubscriptionQueryService queryService, ICurrentUser currentUser)
    {
        private readonly SubscriptionQueryService _queryService = queryService;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Either<IFeatureError, TenantWithLinkedUsers>> Handle(Guid tenantId)
        {
            return await _queryService.GetSelectedSubscriptionForOwnerAsync(_currentUser.Id)
                .BindAsync(s => _queryService.GetSubscriptionLinkedTenantAsync(s.Id,tenantId));
        }
    }
}
