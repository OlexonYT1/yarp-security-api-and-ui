using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Features.Authorizations.Queries.GetAuthorizationForAdmin;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Roles.Results;

namespace UbikLink.Security.Api.Features.Roles.Queries.GetRoleForAdmin
{
    public class GetRoleForAdminEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("admin/roles/{id:guid}",
                async Task<Results<Ok<RoleAdminResult>, ProblemHttpResult>> (
                    [FromRoute] Guid id,
                    [FromServices] GetRoleForAdminHandler handler) =>
                {

                    var result = await handler.Handle(id);

                    return result.Match<Results<Ok<RoleAdminResult>, ProblemHttpResult>>(
                        ok => TypedResults.Ok(ok.MapToRolesAdminResult()),
                        err => CustomTypedResults.Problem(err));
                })
            .WithSummary("Get a role (For admin)")
            .WithDescription("This endpoint get an existing role in the system. (For admin)")
            .WithTags("Roles")
            .ProducesProblem(404);
        }
    }
}
