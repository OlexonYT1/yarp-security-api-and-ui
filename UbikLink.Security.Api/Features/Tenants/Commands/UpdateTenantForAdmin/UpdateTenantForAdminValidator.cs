using FluentValidation;
using UbikLink.Security.Contracts.Tenants.Commands;

namespace UbikLink.Security.Api.Features.Tenants.Commands.UpdateTenantForAdmin
{
    public class UpdateTenantForAdminValidator : AbstractValidator<UpdateTenantCommand>
    {
        public UpdateTenantForAdminValidator()
        {
            RuleFor(x => x.Label)
                .NotEmpty().WithMessage("Label is required.")
                .MaximumLength(100).WithMessage("Label must not exceed 100 characters.");

            RuleFor(x => x.SubscriptionId)
                .NotEmpty().WithMessage("SubscriptionId is required.");

            RuleFor(x => x.IsActivated)
                .NotNull().WithMessage("IsActivated is required.");

            RuleFor(x => x.Version)
                .NotEmpty().WithMessage("Version is required.");
        }
    }
}
