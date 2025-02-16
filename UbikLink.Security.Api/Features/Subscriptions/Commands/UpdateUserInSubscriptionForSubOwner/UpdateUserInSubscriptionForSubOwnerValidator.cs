using FluentValidation;
using UbikLink.Security.Contracts.Subscriptions.Commands;

namespace UbikLink.Security.Api.Features.Subscriptions.Commands.UpdateUserInSubscriptionForSubOwner
{
    public class UpdateUserInSubscriptionForSubOwnerValidator : AbstractValidator<UpdateSubscriptionLinkedUserCommand>
    {
        public UpdateUserInSubscriptionForSubOwnerValidator()
        {
            RuleFor(x => x.Firstname)
                .NotEmpty()
                .WithMessage("Firstname is required.")
                .MaximumLength(100)
                .WithMessage("Firstname must not exceed 100 characters.");

            RuleFor(x => x.Lastname)
                .NotEmpty()
                .WithMessage("Lastname is required.")
                .MaximumLength(100)
                .WithMessage("Lastname must not exceed 100 characters.");
        }
    }
}
