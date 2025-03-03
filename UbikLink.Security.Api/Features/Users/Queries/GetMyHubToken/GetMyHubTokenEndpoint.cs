using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Features.Users.Queries.GetUserMe;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.Security.Api.Features.Users.Queries.GetMyHubToken
{
    public class GetMyHubTokenEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("hubtoken",
                async Task<Results<Ok<string>, ProblemHttpResult>> (
                    [FromServices] GetMyHubTokenHandler handler) =>
                {
                    var result = await handler.Handle();

                    return result.Match<Results<Ok<string>, ProblemHttpResult>>(
                         ok => TypedResults.Ok(ok),
                         err => CustomTypedResults.Problem(err));
                })
            .WithSummary("Get my token to use real time functions")
            .WithDescription("This endpoint get a token to etablish a real time connection.")
            .WithTags("Realtime")
            .ProducesProblem(400);
        }
    }
}
