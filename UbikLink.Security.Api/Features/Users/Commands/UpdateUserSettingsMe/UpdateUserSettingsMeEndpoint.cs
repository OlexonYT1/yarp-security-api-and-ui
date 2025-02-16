using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Features.Users.Services;
using UbikLink.Security.Contracts.Users.Commands;

namespace UbikLink.Security.Api.Features.Users.Commands.UpdateUserSettingsMe
{
    public class UpdateUserSettingsMeEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("me/selecttenant",
                async Task<Results<Ok<Guid?>, ProblemHttpResult>> (
                    [FromBody] SetSettingsUserMeCommand command,
                    [FromServices] UpdateUserSettingsMeHandler handler) =>
                {
                    var result = await handler.Handle(command);

                    return result.Match<Results<Ok<Guid?>, ProblemHttpResult>>(
                        ok => TypedResults.Ok(ok.SelectedTenantId),
                        err => CustomTypedResults.Problem(err));
                })
            .WithSummary("Update user settings, selected tenant Id (me)")
            .WithDescription("This endpoint updates the settings of the current user.")
            .WithTags("Users")
            .ProducesProblem(400);
        }
    }
}
