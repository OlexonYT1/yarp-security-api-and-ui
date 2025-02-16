using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Features.Subscriptions.Commands.UpdateTenantInSubscriptionForSubOwner;
using UbikLink.Security.Contracts.Subscriptions.Commands;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.Security.Api.Features.Subscriptions.Commands.DeleteTenantInSubscriptionForSubOwner
{
    public class DeleteTenantInSubscriptionForSubOwnerEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("subowner/subscriptions/selected/tenants/{Id:guid}",
                async Task<Results<NoContent, ProblemHttpResult>> (
                    [FromRoute] Guid Id,
                    [FromServices] DeleteTenantInSubscriptionForSubOwnerHandler handler) =>
                {
                    var result = await handler.Handle(Id);

                    return result.Match<Results<NoContent, ProblemHttpResult>>(
                        ok => TypedResults.NoContent(),
                        err => CustomTypedResults.Problem(err));

                })
            .WithSummary("Delete a tenant from a subscription")
            .WithDescription("This endpoint delete a tenant for the selected subscription of a sub owner (with all the relations)")
            .WithTags("Subscriptions")
            .ProducesProblem(400)
            .ProducesProblem(404);
        }
    }
}
