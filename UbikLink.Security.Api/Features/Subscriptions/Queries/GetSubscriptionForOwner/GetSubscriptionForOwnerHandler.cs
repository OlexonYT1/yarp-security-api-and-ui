using LanguageExt;
using UbikLink.Common.Api;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Subscriptions.Services;

namespace UbikLink.Security.Api.Features.Subscriptions.Queries.GetSubscriptionForOwner
{
    public class GetSubscriptionForOwnerHandler(SubscriptionQueryService queryService, ICurrentUser currentUser)
    {
        private readonly SubscriptionQueryService _queryService = queryService;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Either<IFeatureError,SubscriptionModel>> Handle()
        {
            return await _queryService.GetSelectedSubscriptionForOwnerAsync(_currentUser.Id);
        }
    }
}
