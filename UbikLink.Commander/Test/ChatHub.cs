using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace UbikLink.Commander.Test
{
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var tenantId = Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;

            if (userId == null || tenantId == null)
            {
                Context.Abort();
                return Task.CompletedTask;
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user,message);
        }
    }

    public interface IChatClient
    {
        Task ReceiveMessage(string user, string message);
    }
}
