using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Features.Authorizations.Commands.AddAuthorizationForAdmin;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Roles.Commands;
using UbikLink.Security.Contracts.Roles.Results;

namespace UbikLink.Security.Api.Features.Roles.Commands.AddRoleForAdmin
{
    public class AddRoleForAdminEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("admin/roles",
                async Task<Results<Created<RoleAdminResult>, ProblemHttpResult>> (
                    [FromBody] AddRoleCommand command,
                    [FromServices] AddRoleForAdminHandler handler,
                    [FromServices] AddRoleForAdminValidator validator) =>
                {
                    var validationResult = await validator.ValidateAsync(command);
                    if (!validationResult.IsValid)
                    {
                        return CustomTypedResults.Problem(validationResult.ToDictionary());
                    }

                    var result = await handler.Handle(command);

                    return result.Match<Results<Created<RoleAdminResult>, ProblemHttpResult>>(
                        ok => TypedResults.Created($"admin/roles/{ok.Id}", ok.MapToRolesAdminResult()),
                        err => CustomTypedResults.Problem(err));

                })
            .WithSummary("Add a role (For admin)")
            .WithDescription("This endpoint adds a new role in the system. (For admin)")
            .WithTags("Roles")
            .ProducesProblem(400)
            .ProducesProblem(409);
        }
    }
}
