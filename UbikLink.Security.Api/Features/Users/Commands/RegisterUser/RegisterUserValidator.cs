using FluentValidation;
using Microsoft.Extensions.Options;
using UbikLink.Common.Api;
using UbikLink.Security.Contracts.Users.Commands;


namespace UbikLink.Security.Api.Features.Users.Commands.RegisterUser
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator(IOptions<AuthRegisterAuthKey> key)
        {
            RuleFor(x => x.AuthorizationKey)
                .NotEmpty().WithMessage("AuthorizationKey is required.")
                .Equal(key.Value.Key).WithMessage("AuthorizationKey is not valid.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is not valid.");

            RuleFor(x => x.Firstname)
                .NotEmpty().WithMessage("Firstname is required.")
                .MaximumLength(50).WithMessage("Firstname must not exceed 50 characters.");

            RuleFor(x => x.Lastname)
                .NotEmpty().WithMessage("Lastname is required.")
                .MaximumLength(50).WithMessage("Lastname must not exceed 50 characters.");
        }
    }
}
