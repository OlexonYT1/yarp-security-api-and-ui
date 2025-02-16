using FluentValidation;
using UbikLink.Security.Contracts.Subscriptions.Commands;

namespace UbikLink.Security.Api.Features.Subscriptions.Commands.UpdateTenantInSubscriptionForSubOwner
{
    public class UpdateTenantInSubscriptionForSubOwnerValidator : AbstractValidator<UpdateSubscriptionLinkedTenantCommand>
    {
        public UpdateTenantInSubscriptionForSubOwnerValidator()
        {
            RuleFor(x => x.Label)
                .NotEmpty()
                .WithMessage("Label is required.")
                .MaximumLength(100)
                .WithMessage("Label must not exceed 100 characters.");

            RuleFor(x => x.SubscriptionId)
                .NotEmpty()
                .WithMessage("SubscriptionId is required.");
        }
    }
}
