using MassTransit;
using UbikLink.Proxy.Services;
using UbikLink.Security.Contracts.Roles.Events;

namespace UbikLink.Proxy.Consumer
{
    public class CleanUsersCacheRoleDeleted(UserService userService) : IConsumer<CleanCacheRoleDeleted>
    {
        private readonly UserService _userService = userService;

        public async Task Consume(ConsumeContext<CleanCacheRoleDeleted> msg)
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
