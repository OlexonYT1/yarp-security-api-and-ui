using Microsoft.AspNetCore.Routing;

namespace UbikLink.Common.Http
{
    public interface IEndpoint
    {
        void MapEndpoint(IEndpointRouteBuilder app);
    }
}
