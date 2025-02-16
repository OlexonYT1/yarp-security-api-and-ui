using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Features.Subscriptions.Queries.GetSubscriptionForOwner;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Subscriptions.Results;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.Security.Api.Features.Subscriptions.Queries.GetSubscriptionAllLinkedTenantsForOwner
{
    public class GetSubscriptionAllLinkedTenantsForOwnerEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("subowner/subscriptions/selected/tenants",
                async Task<Results<Ok<IEnumerable<TenantSubOwnerResult>>, ProblemHttpResult>> (
                    [FromServices] GetSubscriptionAllLinkedTenantsForOwnerHandler handler) =>
                {
                    var result = await handler.Handle();

                    return result.Match<Results<Ok<IEnumerable<TenantSubOwnerResult>>, ProblemHttpResult>>(
                        ok => TypedResults.Ok(ok.MapToTenantSubOwnerResults()),
                        err => CustomTypedResults.Problem(err));
                })
            .WithSummary("Get tenants linked to a subscription for owner")
            .WithDescription("This endpoint retrieves all the tenants linked to a subscription if the user is the owner.")
            .WithTags("Subscriptions")
            .ProducesProblem(404);
        }
    }
}
