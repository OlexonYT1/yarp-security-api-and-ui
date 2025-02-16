using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace UbikLink.Common.Api
{
    internal static class UserMiddlewareErrors
    {
        public static async Task Manage(HttpContext context, string errorMsg)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            Activity? activity = context.Features.Get<IHttpActivityFeature>()?.Activity;

            var problemDetail = new ProblemDetails()
            {
                Status = 400,
                Title = "Missing param in header",
                Detail = errorMsg,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Instance = $"{context.Request.Method} {context.Request.Path}",
                
            };

            problemDetail.Extensions.TryAdd("requestId", context.TraceIdentifier);
            problemDetail.Extensions.TryAdd("traceId", activity?.Id);
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetail,
                                    serializeOptions));
        }

        private static readonly JsonSerializerOptions serializeOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public class MegaAdminUserInHeaderMiddleware(RequestDelegate next)
    {
        
        public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser)
        {
            //x-is-megaadmin
            if (context.Request.Headers.TryGetValue("x-is-megaadmin", out var isMegaAdmin))
            {
                if (bool.TryParse(isMegaAdmin, out var parsedIsMegaAdmin))
                {
                    if (parsedIsMegaAdmin)
                        currentUser.IsMegaAdmin = true;
                    else
                    {
                        await UserMiddlewareErrors.Manage(context, "Invalid x-is-megaadmin");
                        return;
                    }
                }
                else
                {
                    await UserMiddlewareErrors.Manage(context, "Invalid x-is-megaadmin format");
                    return;
                }
            }
            else
            {
                await UserMiddlewareErrors.Manage(context, "x-is-megaadmin is missing");
                return;
            }

            //UserId
            if (context.Request.Headers.TryGetValue("x-user-id", out var userId))
            {
                if (Guid.TryParse(userId, out var parsedUserId))
                {
                    currentUser.Id = parsedUserId;
                }
                else
                {
                    await UserMiddlewareErrors.Manage(context, "Invalid x-user-id format");
                    return;
                }
            }
            else
            {
                await UserMiddlewareErrors.Manage(context, "x-user-id is missing");
                return;
            }

            await next(context);
        }
    }

    public class MeUserInHeaderMiddleware(RequestDelegate next)
    {

        public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser)
        {
            //UserId
            if (context.Request.Headers.TryGetValue("x-user-id", out var userId))
            {
                if (Guid.TryParse(userId, out var parsedUserId))
                {
                    currentUser.Id = parsedUserId;
                }
                else
                {
                    await UserMiddlewareErrors.Manage(context, "Invalid x-user-id format");
                    return;
                }
            }
            else
            {
                await UserMiddlewareErrors.Manage(context, "x-user-id is missing");
                return;
            }

            await next(context);
        }
    }

    public class ProxyTokenInHeaderMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, IOptions<ProxyToken> authorizedToken)
        {
            var token = authorizedToken.Value.Token;

            //UserId
            if (context.Request.Headers.TryGetValue("x-proxy-token", out var proxyToken))
            {
                if (!string.IsNullOrEmpty(proxyToken))
                {
                    if (proxyToken != token)
                    {
                        await UserMiddlewareErrors.Manage(context, "Invalid x-proxy-token");
                        return;
                    }
                    else
                        await next(context);
                }
                else
                {
                    await UserMiddlewareErrors.Manage(context, "Invalid x-proxy-token format");
                    return;
                }
            }
            else
            {
                await UserMiddlewareErrors.Manage(context, "x-proxy-token is missing");
                return;
            }
        }
    }

    public class UserInHeaderMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser)
        {
            //UserId
            if (context.Request.Headers.TryGetValue("x-user-id", out var userId))
            {
                if (Guid.TryParse(userId, out var parsedUserId))
                {
                    currentUser.Id = parsedUserId;
                }
                else
                {
                    await UserMiddlewareErrors.Manage(context, "Invalid x-user-id format");
                    return;
                }
            }
            else
            {
                await UserMiddlewareErrors.Manage(context, "x-user-id is missing");
                return;
            }

            //TenantId
            if (context.Request.Headers.TryGetValue("x-tenant-id", out var tenantId))
            {
                if (Guid.TryParse(tenantId, out var parsedTenantId))
                {
                    currentUser.TenantId = parsedTenantId;
                }
                else
                {
                    await UserMiddlewareErrors.Manage(context, "Invalid x-tenant-id format");
                    return;
                }
            }
            else
            {
                await UserMiddlewareErrors.Manage(context, "x-tenant-id is missing");
                return;
            }

            await next(context);
        }
    }
}
