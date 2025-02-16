using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Features.Authorizations.Commands.UpdateAuthorizationForAdmin;
using UbikLink.Security.Api.Features.Authorizations.Queries.GetAllAuthorizationsForAdmin;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Authorizations.Results;

namespace UbikLink.Security.Api.Features.Authorizations.Queries.GetAuthorizationForAdmin
{
    public class GetAuthorizationForAdminEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("admin/authorizations/{id:guid}",
                async Task<Results<Ok<AuthorizationStandardResult>, ProblemHttpResult>> (
                    [FromRoute] Guid id,
                    [FromServices] GetAuthorizationForAdminHandler handler) =>
                {

                    var result = await handler.Handle(id);

                    return result.Match<Results<Ok<AuthorizationStandardResult>, ProblemHttpResult>>(
                        ok => TypedResults.Ok(ok.MapToAuthorizationStandardResult()),
                        err => CustomTypedResults.Problem(err));
                })
            .WithSummary("Get an authorization (For admin)")
            .WithDescription("This endpoint get an existing authorization in the system. (For admin)")
            .WithTags("Authorizations")
            .ProducesProblem(404);
        }
    }
}
