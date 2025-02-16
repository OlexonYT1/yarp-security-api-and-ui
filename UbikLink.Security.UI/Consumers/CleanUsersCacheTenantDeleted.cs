using MassTransit;
using UbikLink.Common.Frontend.Auth;
using UbikLink.Security.Contracts.Tenants.Events;

namespace UbikLink.Security.UI.Consumers
{
    public class CleanUsersCacheTenantDeleted(UserAndTokenCache cache) : IConsumer<CleanCacheTenantDeleted>
    {
        private readonly UserAndTokenCache _cache = cache;

        public async Task Consume(ConsumeContext<CleanCacheTenantDeleted> msg)
        {
            await _cache.RemoveAllUserInfoInCacheForTenantAsync(msg.Message.TenantId);
        }
    }
}
