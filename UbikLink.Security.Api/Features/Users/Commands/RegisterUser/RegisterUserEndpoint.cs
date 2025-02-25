using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Users.Commands;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.Security.Api.Features.Users.Commands.RegisterUser
{
    public class RegisterUserEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("users/register",
                async Task<Results<Ok<UserRegisterResult>, ProblemHttpResult>> (
                    [FromBody] RegisterUserCommand command,
                    [FromServices] RegisterUserHandler handler,
                    [FromServices] RegisterUserValidator validator) =>
                {
                    var validationResult = await validator.ValidateAsync(command);
                    if (!validationResult.IsValid)
                    {
                        return CustomTypedResults.Problem(validationResult.ToDictionary());
                    }

                    var result = await handler.Handle(command);

                    return TypedResults.Ok(result.MapToUserRegisterResult());

                })
            .WithSummary("Register a user from any app")
            .WithDescription("This endpoint register a user, cannot be called from public, need an authorization code.")
            .WithTags("Users")
            .ProducesProblem(400);
        }
    }
}
