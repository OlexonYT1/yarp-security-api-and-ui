using UbikLink.Security.Contracts.Roles.Results;

namespace UbikLink.Security.UI.Client.Components.Tenant
{
    public class RoleLightUiObj
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public bool Selected { get; set; }
    }

    public static class RoleMappers
    {
        public static RoleLightUiObj ToUiObj(this RoleLightResult role)
        {
            return new RoleLightUiObj
            {
                Id = role.Id,
                Code = role.Code,
                Selected = false
            };
        }

        public static IEnumerable<RoleLightUiObj> ToUiObjs(this IEnumerable<RoleLightResult> roles)
        {
            return roles.Select(ToUiObj);
        }
    }
}
