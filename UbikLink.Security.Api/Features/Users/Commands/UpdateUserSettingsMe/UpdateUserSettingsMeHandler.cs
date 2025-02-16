using LanguageExt;
using UbikLink.Common.Api;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Users.Services;
using UbikLink.Security.Contracts.Users.Commands;

namespace UbikLink.Security.Api.Features.Users.Commands.UpdateUserSettingsMe
{
    public class UpdateUserSettingsMeHandler(UserCommandService userCommandService, ICurrentUser currentUser)
    {
        private readonly UserCommandService _userCommandService = userCommandService;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Either<IFeatureError, UserModel>> Handle(SetSettingsUserMeCommand command)
        {
            return await _userCommandService.GetUserIfTenantLinkExistsAndValidAsync(_currentUser.Id, command.TenantId)
                .BindAsync(user => _userCommandService.SaveAndPublishNewSelectedTenantForUser(user, command.TenantId));
        }
    }
}
