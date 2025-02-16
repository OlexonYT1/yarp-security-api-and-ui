using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Features.Authorizations.Commands.AddAuthorizationForAdmin;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Subscriptions.Commands;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.Security.Api.Features.Subscriptions.Commands.AddTenantInSubscriptionForSubOwner
{
    public class AddTenantInSubscriptionForSubOwnerEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("subowner/subscriptions/selected/tenants",
                async Task<Results<Created<TenantSubOwnerResult>, ProblemHttpResult>> (
                    [FromBody] AddSubscriptionLinkedTenantCommand command,
                    [FromServices] AddTenantInSubscriptionForSubOwnerHandler handler,
                    [FromServices] AddTenantInSubscriptionForSubOwnerValidator validator) =>
                {
                    var validationResult = await validator.ValidateAsync(command);
                    if (!validationResult.IsValid)
                    {
                        return CustomTypedResults.Problem(validationResult.ToDictionary());
                    }

                    var result = await handler.Handle(command);

                    return result.Match<Results<Created<TenantSubOwnerResult>, ProblemHttpResult>>(
                        ok => TypedResults.Created($"subowner/subscriptions/selected/tenants/{ok.Id}", ok.MapToTenantSubOwnerResult()),
                        err => CustomTypedResults.Problem(err));

                })
            .WithSummary("Add a tenant to a subscription")
            .WithDescription("This endpoint adds a new tenant for the selected subscription of a sub owner")
            .WithTags("Subscriptions")
            .ProducesProblem(400);
        }
    }
}
