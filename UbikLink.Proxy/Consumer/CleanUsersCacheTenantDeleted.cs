using MassTransit;
using UbikLink.Proxy.Services;
using UbikLink.Security.Contracts.Tenants.Events;

namespace UbikLink.Proxy.Consumer
{
    public class CleanUsersCacheTenantDeleted(UserService userService) : IConsumer<CleanCacheTenantDeleted>
    {
        private readonly UserService _userService = userService;

        public async Task Consume(ConsumeContext<CleanCacheTenantDeleted> msg)
        {
            await _userService.RemoveAllUserInfoFromCache(msg.Message.TenantId);
        }
    }
}
