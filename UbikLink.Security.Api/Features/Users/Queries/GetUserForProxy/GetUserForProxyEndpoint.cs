using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.Security.Api.Features.Users.Queries.GetUserForProxy
{
    public class GetUserForProxyEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("proxy/users",
                async Task<Results<Ok<UserProxyResult>, ProblemHttpResult>> (
                    [FromQuery] string authid,
                    [FromServices] GetUserForProxyHandler handler) =>
                {
                    var result = await handler.Handle(authid);

                    return result.Match<Results<Ok<UserProxyResult>, ProblemHttpResult>>(
                         ok => TypedResults.Ok(ok.MapToUserProxyResult()),
                         err => CustomTypedResults.Problem(err));
                })
            .WithSummary("Get a user by authid (proxy)")
            .WithDescription("This endpoint get user information (exclusively reserved for proxy calls).")
            .WithTags("Users")
            .ProducesProblem(404);
        }
    }
}
