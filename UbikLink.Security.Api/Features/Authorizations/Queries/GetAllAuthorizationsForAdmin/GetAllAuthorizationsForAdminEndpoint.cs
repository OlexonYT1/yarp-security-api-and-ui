using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Authorizations.Results;

namespace UbikLink.Security.Api.Features.Authorizations.Queries.GetAllAuthorizationsForAdmin
{
    public class GetAllAuthorizationsForAdminEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("admin/authorizations",
                async Task<Ok<IEnumerable<AuthorizationStandardResult>>> (
                    [FromServices] GetAllAuthorizationsForAdminHandler handler) =>
                {
                    var result = await handler.Handle();
                    return TypedResults.Ok(result.MapToAuthorizationStandardResults());
                })
            .WithSummary("Get all authorizations (admin)")
            .WithDescription("This endpoint get all authorizations registred in the system.")
            .WithTags("Authorizations");
        }
    }
}
