using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Features.Authorizations.Commands.DeleteAuthorizationForAdmin;

namespace UbikLink.Security.Api.Features.Roles.Commands.DeleteRoleForAdmin
{
    public class DeleteRoleForAdminEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("admin/roles/{id:guid}",
                async Task<Results<NoContent, ProblemHttpResult>> (
                    [FromRoute] Guid id,
                    [FromServices] DeleteRoleForAdminHandler handler) =>
                {
                    var result = await handler.Handle(id);

                    return result.Match<Results<NoContent, ProblemHttpResult>>(
                        _ => TypedResults.NoContent(),
                        err => CustomTypedResults.Problem(err));
                })
            .WithSummary("Delete a role (For admin)")
            .WithDescription("This endpoint deletes a role from the system. (For admin)")
            .WithTags("Roles")
            .ProducesProblem(400)
            .ProducesProblem(404);
        }
    }
}
