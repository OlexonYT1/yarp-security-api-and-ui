using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.Security.Api.Features.Subscriptions.Queries.GetSubscriptionAllLinkedUsersForOwner
{
    public class GetSubscriptionAllLinkedUsersForOwnerEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("subowner/subscriptions/selected/users",
                async Task<Results<Ok<IEnumerable<UserSubOwnerResult>>, ProblemHttpResult>> (
                    [FromServices] GetSubscriptionAllLinkedUsersForOwnerHandler handler) =>
                {
                    var result = await handler.Handle();

                    return result.Match<Results<Ok<IEnumerable<UserSubOwnerResult>>, ProblemHttpResult>>(
                        ok => TypedResults.Ok(ok.MapToUserSubOwnerResults()),
                        err => CustomTypedResults.Problem(err));
                })
            .WithSummary("Get users linked to a subscription for owner")
            .WithDescription("This endpoint retrieves all the users linked to a subscription if the user is the owner.")
            .WithTags("Subscriptions")
            .ProducesProblem(404);
        }
    }
}
