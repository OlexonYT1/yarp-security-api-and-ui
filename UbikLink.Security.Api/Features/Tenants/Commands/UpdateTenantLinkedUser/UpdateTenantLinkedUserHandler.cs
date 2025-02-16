using LanguageExt;
using UbikLink.Common.Api;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Features.Tenants.Services;
using UbikLink.Security.Api.Features.Tenants.Services.Poco;
using UbikLink.Security.Contracts.Users.Commands;

namespace UbikLink.Security.Api.Features.Tenants.Commands.UpdateTenantLinkedUser
{
    //TODO: this part is more than messy....
    public class UpdateTenantLinkedUserHandler(TenantCommandService commandService, ICurrentUser currentUser)
    {
        private readonly TenantCommandService _commandService = commandService;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<Either<IFeatureError, bool>> Handle(UpdateUserWithTenantRoleIdsCommand command,
            Guid tenantId,
            Guid userId)
        {
            return _currentUser.TenantId != tenantId
                ? new ResourceNotFoundError("Tenant", new Dictionary<string, string>()
                 {
                     { "Id", tenantId.ToString() }
                 })
                : await _commandService.GetTenantByIdAsync(tenantId)
                 .BindAsync(t => _commandService.GetTenantUserWithRolesAsync(t, userId))
                 .BindAsync(tu => _commandService.PrepareRoleIdsForAttachAndDetachFromUserUpdAsync(tu.Tenant,
                    tu.User,
                    command.TenantRoleIds))
                 .BindAsync(x => _commandService.UpdateTenantUserRole(x.tenant,
                    x.user,
                    x.TenantUserId,
                    x.RoleIdsForInsert,
                    x.RoleIdsForDel));
        }
    }
}
