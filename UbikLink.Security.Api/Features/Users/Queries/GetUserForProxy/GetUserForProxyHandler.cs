using LanguageExt;
using Microsoft.EntityFrameworkCore;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Users.Services;
using UbikLink.Security.Api.Features.Users.Services.Poco;

namespace UbikLink.Security.Api.Features.Users.Queries.GetUserForProxy
{
    public class GetUserForProxyHandler(UserQueryService query)
    {
        private readonly UserQueryService _query = query;

        public async Task<Either<IFeatureError,UserWithSubscriptionInfo>> 
            Handle(string oAuthId)
        {
            return await _query.GetUserByAuthId(oAuthId)
                    .BindAsync(_query.IsActivatedInSelectedSubscription)
                    .BindAsync(_query.FillOwnedSubscribtionIdsAsync)
                    .BindAsync(_query.GetTenantRolesAndAuthorizationsAsync);
                    
        }
    }
}
