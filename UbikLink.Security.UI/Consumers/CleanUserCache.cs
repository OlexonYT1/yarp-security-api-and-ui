using MassTransit;
using UbikLink.Common.Frontend.Auth;
using UbikLink.Security.Contracts.Tenants.Events;
using UbikLink.Security.Contracts.Users.Events;

namespace UbikLink.Security.UI.Consumers
{
    public class CleanUserCache(UserAndTokenCache cache) : IConsumer<CleanCacheForUserRequestSent>
    {
        private readonly UserAndTokenCache _cache = cache;

        public async Task Consume(ConsumeContext<CleanCacheForUserRequestSent> msg)
        {
            await _cache.RemoveUserInfoInCacheAsync(msg.Message.AuthId);
        }
    }
}
