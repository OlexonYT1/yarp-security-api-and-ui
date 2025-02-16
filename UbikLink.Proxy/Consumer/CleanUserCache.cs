using MassTransit;
using UbikLink.Common.Frontend.Auth;
using UbikLink.Proxy.Services;
using UbikLink.Security.Contracts.Users.Events;

namespace UbikLink.Proxy.Consumer
{
    public class CleanUserCache(UserService userService) : IConsumer<CleanCacheForUserRequestSent>
    {
        private readonly UserService _userService = userService;

        public async Task Consume(ConsumeContext<CleanCacheForUserRequestSent> msg)
        {
            await _userService.RemoveUserInfoFromCacheAsync(msg.Message.AuthId);
        }
    }
}
