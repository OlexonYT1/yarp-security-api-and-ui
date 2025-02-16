using LanguageExt;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Features.Roles.Services;
using UbikLink.Security.Contracts.Roles.Commands;

namespace UbikLink.Security.Api.Features.Roles.Commands.BatchDeleteRoleForAdmin
{
    public class BatchDeleteRoleForAdminHandler(RoleCommandService commandService)
    {
        private readonly RoleCommandService _commandService = commandService;

        public async Task<Either<IFeatureError, bool>> Handle(BatchDeleteRoleCommand command)
        {
            return await _commandService.GetRolesByIdsForAdmin(command.RoleIds)
                .BindAsync(_commandService.DeleteRolesRangeInDbAsync);
        }
    }
}
