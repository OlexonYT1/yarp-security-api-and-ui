using LanguageExt;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Authorizations.Services;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Authorizations.Commands;

namespace UbikLink.Security.Api.Features.Authorizations.Commands.UpdateAuthorizationForAdmin
{
    public class UpdateAuthorizationForAdminHandler(AuthorizationCommandService commandService)
    {
        private readonly AuthorizationCommandService _commandService = commandService;

        public async Task<Either<IFeatureError, AuthorizationModel>> Handle(UpdateAuthorizationCommand command, Guid currentId)
        {
            var upd = command.MapToAuthorization(currentId);
            
            return await _commandService.ValidateIfExistsIdAsync(currentId)
                .BindAsync(x=> _commandService.MapInDbContextAsync(x,upd))
                .BindAsync(_commandService.ValidateIfNotAlreadyExistsWithOtherIdAsync)
                .BindAsync(_commandService.UpdateAuthorizationInDbAsync);
        }
    }
}
