using FluentValidation;
using UbikLink.Security.Contracts.Roles.Commands;

namespace UbikLink.Security.Api.Features.Roles.Commands.UpdateRoleForAdmin
{
    public class UpdateRoleForAdminValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleForAdminValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        }
    }
}
