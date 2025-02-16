using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Common.Frontend.Auth;
using Yarp.ReverseProxy.Transforms;

namespace UbikLink.Common.Http
{
    public static class WasmProxy
    {
        public static IApplicationBuilder MapWasmProxy(
            this WebApplication app,
            string serviceDiscoveryUrl)
        {
            app.MapForwarder(
                "api/{**catch-all}",
                serviceDiscoveryUrl,
                transformBuilderContext =>
                {
                    transformBuilderContext.AddPathRemovePrefix("/api");
                    transformBuilderContext.AddRequestTransform(
                        async requestTransformContext =>
                        {
                            var userService = requestTransformContext.HttpContext
                                .RequestServices.GetRequiredService<ClientUserService>();

                            var usertoken = await userService.GetCurrentUserTokenAsync();

                            requestTransformContext.ProxyRequest.Headers.Clear();
                            requestTransformContext.ProxyRequest.Headers.Add("Authorization", $"Bearer {usertoken}");
                        });
                });
            return app;
        }
    }
}
