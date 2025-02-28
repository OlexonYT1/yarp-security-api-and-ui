using LanguageExt;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using UbikLink.Common.Api;
using UbikLink.Common.Db;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Users.Services;
using UbikLink.Security.Contracts.Users.Commands;

namespace UbikLink.Security.Api.Features.Users.Commands.OnboardMeSimple
{
    public class OnboardMeSimpleHandler(UserCommandService commandService
        , ICurrentUser currentUser
        , IOptions<AuthRegisterAuthKey> config)
    {
        private readonly UserCommandService _commandService = commandService;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly bool _checkTrueActivation = config.Value.EmailActivationActivated;

        public async Task<Either<IFeatureError, UserModel>> Handle(OnboardMeSimpleCommand command)
        {
            return await _commandService.GetUserIfNotAlreadyOnBoardedAsync(_currentUser.Id)
                .BindAsync(u => _commandService.ActivateUserEmailInContext(u, _checkTrueActivation, command.ActivationCode))
                .BindAsync(_commandService.SimpleOnboardingAsync);
        }
    }
}
