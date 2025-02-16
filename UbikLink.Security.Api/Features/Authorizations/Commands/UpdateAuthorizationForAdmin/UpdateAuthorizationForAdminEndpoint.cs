using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Authorizations.Results;

namespace UbikLink.Security.Api.Features.Authorizations.Commands.UpdateAuthorizationForAdmin
{
    public class UpdateAuthorizationForAdminEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("admin/authorizations/{id:guid}",
                async Task<Results<Ok<AuthorizationStandardResult>, ProblemHttpResult>> (
                    [FromRoute] Guid id,
                    [FromBody] UpdateAuthorizationCommand command,
                    [FromServices] UpdateAuthorizationForAdminHandler handler,
                    [FromServices] UpdateAuthorizationForAdminValidator validator) =>
                {
                    var validationResult = await validator.ValidateAsync(command);
                    if (!validationResult.IsValid)
                    {
                        return CustomTypedResults.Problem(validationResult.ToDictionary());
                    }

                    var result = await handler.Handle(command,id);

                    return result.Match<Results<Ok<AuthorizationStandardResult>, ProblemHttpResult>>(
                        ok => TypedResults.Ok(ok.MapToAuthorizationStandardResult()),
                        err => CustomTypedResults.Problem(err));
                })
            .WithSummary("Update an authorization (For admin)")
            .WithDescription("This endpoint updates an existing authorization in the system. (For admin)")
            .WithTags("Authorizations")
            .ProducesProblem(400)
            .ProducesProblem(409);
        }
    }
}
