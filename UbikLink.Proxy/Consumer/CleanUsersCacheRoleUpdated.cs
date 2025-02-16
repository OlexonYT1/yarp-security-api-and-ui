using MassTransit;
using UbikLink.Proxy.Services;
using UbikLink.Security.Contracts.Roles.Events;
using UbikLink.Security.Contracts.Tenants.Events;

namespace UbikLink.Proxy.Consumer
{
    public class CleanUsersCacheRoleUpdated(UserService userService) : IConsumer<CleanCacheRoleUpdated>
    {
        private readonly UserService _userService = userService;

        public async Task Consume(ConsumeContext<CleanCacheRoleUpdated> msg)
        {
            var op = msg.Message;

            if (op.TenantId == null)
            {
                await _userService.RemoveAllUserInfoFromCache();
            }
            else
            {
                await _userService.RemoveAllUserInfoFromCache((Guid)op.TenantId);
            }
        }
    }
}
