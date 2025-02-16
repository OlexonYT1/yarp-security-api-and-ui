using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;

namespace UbikLink.Security.Api.Features.Authorizations.Commands.DeleteAuthorizationForAdmin
{
    public class DeleteAuthorizationForAdminEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("admin/authorizations/{id:guid}",
                async Task<Results<NoContent, ProblemHttpResult>> (
                    [FromRoute] Guid id,
                    [FromServices] DeleteAuthorizationForAdminHandler handler) =>
                {
                    var result = await handler.Handle(id);

                    return result.Match<Results<NoContent, ProblemHttpResult>>(
                        _ => TypedResults.NoContent(),
                        err => CustomTypedResults.Problem(err));
                })
            .WithSummary("Delete an authorization (For admin)")
            .WithDescription("This endpoint deletes an authorization from the system. (For admin)")
            .WithTags("Authorizations")
            .ProducesProblem(400)
            .ProducesProblem(404);
        }
    }
}
