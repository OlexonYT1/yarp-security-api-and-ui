using MassTransit;
using UbikLink.Common.Frontend.Auth;
using UbikLink.Security.Contracts.Tenants.Events;
using UbikLink.Security.Contracts.Users.Events;

namespace UbikLink.Security.UI.Consumers
{
    public class CleanUsersCacheTenantUpdated(UserAndTokenCache cache) : IConsumer<CleanCacheTenantUpdated>
    {
        private readonly UserAndTokenCache _cache = cache;

        public async Task Consume(ConsumeContext<CleanCacheTenantUpdated> msg)
        {
            await _cache.RemoveAllUserInfoInCacheForTenantAsync(msg.Message.TenantId);
        }
    }
}

