using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Features.Users.Queries.GetUserForProxy;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.Security.Api.Features.Users.Queries.GetUserMe
{
    public class GetUserMeEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("me/authinfo",
                async Task<Results<Ok<UserMeResult>, ProblemHttpResult>> (
                    [FromServices] GetUserMeHandler handler) =>
                {
                    var result = await handler.Handle();

                    return result.Match<Results<Ok<UserMeResult>, ProblemHttpResult>>(
                         ok => TypedResults.Ok(ok.MapToUserMeResult()),
                         err => CustomTypedResults.Problem(err));
                })
            .WithSummary("Get me user info")
            .WithDescription("This endpoint get user information (exclusively reserved for me calls).")
            .WithTags("Users")
            .ProducesProblem(404);
        }
    }
}
