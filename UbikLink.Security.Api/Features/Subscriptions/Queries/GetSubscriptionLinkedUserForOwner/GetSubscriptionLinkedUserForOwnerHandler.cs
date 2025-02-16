using LanguageExt;
using UbikLink.Common.Api;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Features.Subscriptions.Services.Poco;
using UbikLink.Security.Api.Features.Subscriptions.Services;
using UbikLink.Security.Api.Data.Models;

namespace UbikLink.Security.Api.Features.Subscriptions.Queries.GetSubscriptionLinkedUserForOwner
{
    public class GetSubscriptionLinkedUserForOwnerHandler(SubscriptionQueryService queryService, ICurrentUser currentUser)
    {
        private readonly SubscriptionQueryService _queryService = queryService;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Either<IFeatureError, (UserModel User, SubscriptionUserModel SubInfo)>> 
            Handle(Guid tenantId)
        {
            return await _queryService.GetSelectedSubscriptionForOwnerAsync(_currentUser.Id)
                .BindAsync(s => _queryService.GetSubscriptionLinkedUserWithInfoAsync(s.Id, tenantId));
        }
    }
}
