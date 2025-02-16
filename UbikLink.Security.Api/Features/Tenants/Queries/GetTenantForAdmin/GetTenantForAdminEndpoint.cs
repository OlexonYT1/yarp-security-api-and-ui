using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.Security.Api.Features.Tenants.Queries.GetTenantForAdmin
{
    public class GetTenantForAdminEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("admin/tenants/{id}",
                async Task<Results<Ok<TenantStandardResult>, ProblemHttpResult>> (
                    [FromRoute] Guid id,
                    [FromServices] GetTenantForAdminHandler handler) =>
                {
                    var result = await handler.Handle(id);

                    return result.Match<Results<Ok<TenantStandardResult>, ProblemHttpResult>>(
                         ok => TypedResults.Ok(ok.MapToTenantStandardResult()),
                         err => CustomTypedResults.Problem(err));
                })
            .WithSummary("Get a tenant by id (admin)")
            .WithDescription("This endpoint get a tenant by id.")
            .WithTags("Tenants")
            .ProducesProblem(404);
        }
    }
}
