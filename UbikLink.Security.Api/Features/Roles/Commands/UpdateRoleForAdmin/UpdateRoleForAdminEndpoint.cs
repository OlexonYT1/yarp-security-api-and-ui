using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Features.Roles.Services.Poco;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Roles.Commands;
using UbikLink.Security.Contracts.Roles.Results;

namespace UbikLink.Security.Api.Features.Roles.Commands.UpdateRoleForAdmin
{
    public class UpdateRoleForAdminEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("admin/roles/{id:guid}",
                async Task<Results<Ok<RoleAdminResult>, ProblemHttpResult>> (
                    [FromRoute] Guid id,
                    [FromBody] UpdateRoleCommand command,
                    [FromServices] UpdateRoleForAdminHandler handler,
                    [FromServices] UpdateRoleForAdminValidator validator) =>
                {
                    var validationResult = await validator.ValidateAsync(command);
                    if (!validationResult.IsValid)
                    {
                        return CustomTypedResults.Problem(validationResult.ToDictionary());
                    }

                    var result = await handler.Handle(command, id);

                    return result.Match<Results<Ok<RoleAdminResult>, ProblemHttpResult>>(
                        ok => TypedResults.Ok(ok.MapToRolesAdminResult()),
                        err => CustomTypedResults.Problem(err));
                })
            .WithSummary("Update a role (For admin)")
            .WithDescription("This endpoint updates an existing role in the system. (For admin)")
            .WithTags("Roles")
            .ProducesProblem(400)
            .ProducesProblem(409);
        }
    }
}
