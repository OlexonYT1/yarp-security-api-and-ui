using FluentValidation;
using UbikLink.Security.Contracts.Tenants.Commands;

namespace UbikLink.Security.Api.Features.Tenants.Commands.AddTenantForAdmin
{
    public class AddTenantForAdminValidator : AbstractValidator<AddTenantCommand>
    {
        public AddTenantForAdminValidator()
        {
            RuleFor(x => x.Label)
                .NotEmpty()
                .WithMessage("Label is required.")
                .MaximumLength(100)
                .WithMessage("Label must not exceed 100 characters.");

            RuleFor(x => x.SubscriptionId)
                .NotEmpty().WithMessage("SubscriptionId is required.");
        }
    }
}
