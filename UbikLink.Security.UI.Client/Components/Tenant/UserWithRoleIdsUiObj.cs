using UbikLink.Security.Contracts.Users.Commands;
using UbikLink.Security.Contracts.Users.Results;
using UbikLink.Security.UI.Client.Components.Subscription;

namespace UbikLink.Security.UI.Client.Components.Tenant
{
    public class UserWithRoleIdsUiObj
    {
        public Guid Id { get; set; }
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Email { get; set; }
        public Guid Version { get; set; }
        public bool Selected { get; set; } = false;
        public List<Guid> RoleIds { get; set; } = [];
    }
    public static class UserMappers
    {
        public static UserWithRoleIdsUiObj ToUiObj(this UserWithTenantRoleIdsResult user)
        {
            return new UserWithRoleIdsUiObj
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Version = user.Version,
                Selected = false,
                RoleIds = user.TenantRoleIds,
            };
        }
        public static IEnumerable<UserWithRoleIdsUiObj> ToUiObjs(this IEnumerable<UserWithTenantRoleIdsResult> users)
        {
            return users.Select(ToUiObj);
        }

        public static UpdateUserWithTenantRoleIdsCommand MapToUpdateUserWithTenantRoleIdsCommand(List<Guid> roleIds)
        {
            return new UpdateUserWithTenantRoleIdsCommand
            {
                TenantRoleIds = roleIds
            };
        }

    }
}
