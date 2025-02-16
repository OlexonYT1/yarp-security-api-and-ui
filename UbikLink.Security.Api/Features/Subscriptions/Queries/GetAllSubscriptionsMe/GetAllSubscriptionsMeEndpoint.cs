using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Subscriptions.Results;

namespace UbikLink.Security.Api.Features.Subscriptions.Queries.GetAllSubscriptionsMe
{
    public class GetAllSubscriptionsMeEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("me/subscriptions",
                async Task<Results<Ok<IEnumerable<SubscriptionStandardResult>>, ProblemHttpResult>> (
                    [FromServices] GetAllSubscriptionsMeHandler handler) =>
                {
                    var result = await handler.Handle();

                    return TypedResults.Ok(result.MapToSubscriptionStandardResults());
                })
            .WithSummary("Get all subscriptions for me requests")
            .WithDescription("This endpoint retrieves all subscriptions for a specific user (me).")
            .WithTags("Subscriptions");
        }
    }
}
