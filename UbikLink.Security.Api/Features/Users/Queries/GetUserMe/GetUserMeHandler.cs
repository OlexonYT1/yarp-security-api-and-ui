using LanguageExt;
using UbikLink.Common.Api;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Users.Services;
using UbikLink.Security.Api.Features.Users.Services.Poco;

namespace UbikLink.Security.Api.Features.Users.Queries.GetUserMe
{
    public class GetUserMeHandler(UserQueryService query, ICurrentUser currentUser)
    {
        private readonly UserQueryService _query = query;
        public async Task<Either<IFeatureError, UserWithSubscriptionInfo>> Handle()
        {
            return await _query.GetUserById(currentUser.Id)
                    .BindAsync(_query.IsActivatedInSelectedSubscription)
                    .BindAsync(_query.FillOwnedSubscribtionIdsAsync)
                    .BindAsync(_query.GetTenantRolesAndAuthorizationsAsync);
        }
    }
}
