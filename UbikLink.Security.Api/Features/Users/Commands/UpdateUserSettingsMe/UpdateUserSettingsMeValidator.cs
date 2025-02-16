using FluentValidation;
using UbikLink.Security.Contracts.Users.Commands;

namespace UbikLink.Security.Api.Features.Users.Commands.UpdateUserSettingsMe
{
    public class UpdateUserSettingsMeValidator : AbstractValidator<SetSettingsUserMeCommand>
    {
        public UpdateUserSettingsMeValidator()
        {
            RuleFor(x => x.TenantId)
                .NotEmpty().WithMessage("TenantId is required.");
        }
    }
}
