using LanguageExt;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Authorizations.Services;
using UbikLink.Security.Api.Features.Roles.Services;
using UbikLink.Security.Api.Features.Roles.Services.Poco;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Roles.Commands;

namespace UbikLink.Security.Api.Features.Roles.Commands.AddRoleForAdmin
{
    public class AddRoleForAdminHandler(RoleCommandService commandService)
    {
        private readonly RoleCommandService _commandService = commandService;

        public async Task<Either<IFeatureError, RoleWithAuthorizationIds>> Handle(AddRoleCommand command)
        {
            var current = command.MapToRoleWithAuthIds();

            return await _commandService.ValidateIfNotAlreadyExistsForAdminAsync(current.Role)
                    .BindAsync(r => _commandService.ValidateAuthorizationIdsList(r, current.AuthorizationIds))
                    .BindAsync(ra => _commandService.AddRoleForAdminInDbAsync(ra.Role,ra.AuthorizationIds));
        }
    }
}
