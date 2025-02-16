using LanguageExt;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Authorizations.Services;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Authorizations.Commands;

namespace UbikLink.Security.Api.Features.Authorizations.Commands.AddAuthorizationForAdmin
{
    public class AddAuthorizationForAdminHandler(AuthorizationCommandService commandService)
    {
        private readonly AuthorizationCommandService _commandService = commandService;

        public async Task<Either<IFeatureError, AuthorizationModel>> Handle(AddAuthorizationCommand command)
        {
            var current = command.MapToAuthorization();

            return await _commandService.ValidateIfNotAlreadyExistsAsync(current)
                    .BindAsync(_commandService.AddAuthorizationInDbAsync);
        }
    }
}
