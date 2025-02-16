using LanguageExt;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Features.Authorizations.Services;

namespace UbikLink.Security.Api.Features.Authorizations.Commands.DeleteAuthorizationForAdmin
{
    public class DeleteAuthorizationForAdminHandler(AuthorizationCommandService commandService)
    {
        private readonly AuthorizationCommandService _commandService = commandService;

        public async Task<Either<IFeatureError, bool>> Handle(Guid id)
        {
            return await _commandService.ValidateIfExistsIdAsync(id)
                    .BindAsync(_commandService.DeleteAuthorizationInDbAsync);
        }
    }
}
