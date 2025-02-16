using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Features.Tenants.Queries.GetTenantForAdmin;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Roles.Results;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.Security.Api.Features.Tenants.Queries.GetTenantRoles
{
    public class GetTenantRolesEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("tenants/{id:guid}/roles",
                async Task<Results<Ok<IEnumerable<RoleLightResult>>, ProblemHttpResult>> (
                    [FromRoute] Guid id,
                    [FromServices] GetTenantRolesHandler handler) =>
                {
                    var result = await handler.Handle(id);

                    return result.Match<Results<Ok<IEnumerable<RoleLightResult>>, ProblemHttpResult>>(
                         ok => TypedResults.Ok(ok.MapToRoleLightResults()),
                         err => CustomTypedResults.Problem(err));
                })
            .WithSummary("Get all possible roles for a tenant.")
            .WithDescription("This endpoint get a all possible roles that can be asigned to a specific tenants..")
            .WithTags("Tenants")
            .ProducesProblem(404);
        }
    }
}
