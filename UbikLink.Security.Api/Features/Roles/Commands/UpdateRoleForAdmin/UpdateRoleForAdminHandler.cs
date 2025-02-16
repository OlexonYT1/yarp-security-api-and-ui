using LanguageExt;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Authorizations.Services;
using UbikLink.Security.Api.Features.Roles.Services;
using UbikLink.Security.Api.Features.Roles.Services.Poco;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Roles.Commands;

namespace UbikLink.Security.Api.Features.Roles.Commands.UpdateRoleForAdmin
{
    public class UpdateRoleForAdminHandler(RoleCommandService commandService)
    {
        private readonly RoleCommandService _commandService = commandService;

        public async Task<Either<IFeatureError, RoleWithAuthorizationIds>> Handle(UpdateRoleCommand command, Guid currentId)
        {
            var upd = command.MapToRole(currentId);

            return await _commandService.ValidateIfExistsIdAsync(currentId)
                .BindAsync(r => _commandService.MapInDbContextAsync(r, upd))
                .BindAsync(_commandService.ValidateIfNotAlreadyExistsWithOtherIdForAdminAsync)
                .BindAsync(r => _commandService.ValidateAuthorizationIdsList(r, command.AuthorizationIds))
                .BindAsync(ra => _commandService.PrepareAuthorizationIdsForAttachAndDetachForUpdAsync(ra.Role, ra.AuthorizationIds))
                .BindAsync(rad => _commandService.UpdateRoleForAdminAsync(rad.Role, rad.AttachIds, rad.DetachIds));
        }
    }
}
