using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.Security.Api.Features.Tenants.Queries.GetAllTenantsMe
{
    public class GetAllTenantsMeEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("me/tenants",
                async Task<Results<Ok<IEnumerable<TenantStandardResult>>, ProblemHttpResult>> (
                    [FromServices] GetAllTenantsMeHandler handler) =>
                {
                    var result = await handler.Handle();

                    return TypedResults.Ok(result.MapToTenantStandardResults());
                })
            .WithSummary("Get all tenants for the current user")
            .WithDescription("This endpoint gets all tenants for the current user.")
            .WithTags("Tenants");
        }
    }
}
