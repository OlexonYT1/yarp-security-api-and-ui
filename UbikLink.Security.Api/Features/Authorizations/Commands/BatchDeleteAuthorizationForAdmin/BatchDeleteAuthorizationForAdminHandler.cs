using LanguageExt;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Features.Authorizations.Services;
using UbikLink.Security.Contracts.Authorizations.Commands;

namespace UbikLink.Security.Api.Features.Authorizations.Commands.BatchDeleteAuthorizationForAdmin
{
    public class BatchDeleteAuthorizationForAdminHandler(AuthorizationCommandService commandService)
    {
        private readonly AuthorizationCommandService _commandService = commandService;

        public async Task<Either<IFeatureError, bool>> Handle(BatchDeleteAuthorizationCommand command)
        {
            return await _commandService.GetAuthorizationsByIds(command.AuthorizationIds)
                .BindAsync(_commandService.DeleteAuthorizationsRangeInDbAsync);
        }
    }
}
