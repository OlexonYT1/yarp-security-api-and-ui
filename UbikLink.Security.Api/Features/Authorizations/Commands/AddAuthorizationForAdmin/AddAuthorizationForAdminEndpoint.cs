using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Authorizations.Results;

namespace UbikLink.Security.Api.Features.Authorizations.Commands.AddAuthorizationForAdmin
{
    public class AddAuthorizationForAdminEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("admin/authorizations",
                async Task<Results<Created<AuthorizationStandardResult>, ProblemHttpResult>> (
                    [FromBody] AddAuthorizationCommand command,
                    [FromServices] AddAuthorizationForAdminHandler handler,
                    [FromServices] AddAuthorizationForAdminValidator validator) =>
                {
                    var validationResult = await validator.ValidateAsync(command);
                    if (!validationResult.IsValid)
                    {
                        return CustomTypedResults.Problem(validationResult.ToDictionary());
                    }

                    var result = await handler.Handle(command);

                    return result.Match<Results<Created<AuthorizationStandardResult>, ProblemHttpResult>>(
                        ok => TypedResults.Created($"admin/authorizations/{ok.Id}", ok.MapToAuthorizationStandardResult()),
                        err => CustomTypedResults.Problem(err));

                })
            .WithSummary("Add an authorization (For admin)")
            .WithDescription("This endpoint adds a new authorization in the system. (For admin)")
            .WithTags("Authorizations")
            .ProducesProblem(400)
            .ProducesProblem(409);
        }
    }
}
