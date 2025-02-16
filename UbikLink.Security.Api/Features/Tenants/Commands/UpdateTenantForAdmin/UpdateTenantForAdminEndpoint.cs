using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Tenants.Commands;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.Security.Api.Features.Tenants.Commands.UpdateTenantForAdmin
{
    public class UpdateTenantForAdminEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("admin/tenants/{id:guid}",
                async Task<Results<Ok<TenantStandardResult>, ProblemHttpResult>> (
                    [FromRoute] Guid id,
                    [FromBody] UpdateTenantCommand command,
                    [FromServices] UpdateTenantForAdminHandler handler,
                    [FromServices] UpdateTenantForAdminValidator validator) =>
                {
                    var validationResult = await validator.ValidateAsync(command);
                    if (!validationResult.IsValid)
                    {
                        return CustomTypedResults.Problem(validationResult.ToDictionary());
                    }

                    var result = await handler.Handle(command, id);

                    return result.Match<Results<Ok<TenantStandardResult>, ProblemHttpResult>>(
                        ok => TypedResults.Ok(ok.MapToTenantStandardResult()),
                        err => CustomTypedResults.Problem(err));

                })
            .WithSummary("Update a tenant (For admin)")
            .WithDescription("This endpoint updates an existing tenant in the system. (For admin)")
            .WithTags("Tenants")
            .ProducesProblem(400)
            .ProducesProblem(404)
            .ProducesProblem(409);
        }
    }
}

