using MassTransit;
using UbikLink.Proxy.Services;
using UbikLink.Security.Contracts.Tenants.Events;
using UbikLink.Security.Contracts.Users.Events;

namespace UbikLink.Proxy.Consumer
{
    public class CleanUsersCacheTenantUpdated(UserService userService) : IConsumer<CleanCacheTenantUpdated>
    {
        private readonly UserService _userService = userService;

        public async Task Consume(ConsumeContext<CleanCacheTenantUpdated> msg)
        {
            await _userService.RemoveAllUserInfoFromCache(msg.Message.TenantId);
        }
    }
}
