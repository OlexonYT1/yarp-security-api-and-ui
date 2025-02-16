using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Features.Subscriptions.Commands.AddTenantInSubscriptionForSubOwner;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Subscriptions.Commands;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.Security.Api.Features.Subscriptions.Commands.UpdateTenantInSubscriptionForSubOwner
{
    public class UpdateTenantInSubscriptionForSubOwnerEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("subowner/subscriptions/selected/tenants/{Id:guid}",
                async Task<Results<Ok<TenantSubOwnerResult>, ProblemHttpResult>> (
                    [FromBody] UpdateSubscriptionLinkedTenantCommand command,
                    [FromRoute] Guid Id,
                    [FromServices] UpdateTenantInSubscriptionForSubOwnerHandler handler,
                    [FromServices] UpdateTenantInSubscriptionForSubOwnerValidator validator) =>
                {
                    var validationResult = await validator.ValidateAsync(command);
                    if (!validationResult.IsValid)
                    {
                        return CustomTypedResults.Problem(validationResult.ToDictionary());
                    }

                    var result = await handler.Handle(command,Id);

                    return result.Match<Results<Ok<TenantSubOwnerResult>, ProblemHttpResult>>(
                        ok => TypedResults.Ok(ok.MapToTenantSubOwnerResult()),
                        err => CustomTypedResults.Problem(err));

                })
            .WithSummary("Update a tenant to a subscription")
            .WithDescription("This endpoint update a tenant for the selected subscription of a sub owner")
            .WithTags("Subscriptions")
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(409);
        }
    }
}
