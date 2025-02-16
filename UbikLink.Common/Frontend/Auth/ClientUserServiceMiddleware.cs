using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbikLink.Common.Frontend.Auth
{
    public class ClientUserServiceMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

        public async Task InvokeAsync(HttpContext context, ClientUserService service)
        {
            service.SetUser(context.User);
            await _next(context);
        }
    }
}
