using System.ComponentModel.DataAnnotations;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Roles.Commands;
using UbikLink.Security.Contracts.Roles.Results;
using UbikLink.Security.UI.Client.Components.Authorizations;

namespace UbikLink.Security.UI.Client.Components.Roles
{
    public class RoleUiObj
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Code too long (50 character limit).")]
        public string Code { get; set; } = default!;

        [StringLength(500, ErrorMessage = "Description too long (500 character limit).")]
        public string? Description { get; set; }

        public bool Selected { get; set; } = false;
        public List<Guid> AuthorizationIds { get; set; } = [];
        public Guid Version { get; set; }
    }

    public static class RoleUiObjMappers
    {
        public static RoleUiObj MapToRoleUiObj(this RoleAdminResult result)
        {
            return new RoleUiObj
            {
                Id = result.Id,
                Code = result.Code,
                Description = result.Description,
                Version = result.Version,
                AuthorizationIds = result.AuthorizationIds
            };
        }

        public static IEnumerable<RoleUiObj> MapToRoleUiObjs(this IEnumerable<RoleAdminResult> results)
        {
            return results.Select(MapToRoleUiObj);
        }

        public static RoleAdminResult MapToRoleAdminResult(this RoleUiObj uiObj)
        {
            return new RoleAdminResult
            {
                Id = uiObj.Id,
                Code = uiObj.Code,
                Description = uiObj.Description,
                Version = uiObj.Version,
                AuthorizationIds = uiObj.AuthorizationIds
            };
        }

        public static IEnumerable<RoleAdminResult> MapToRoleAdminResults(this IEnumerable<RoleUiObj> uiObjs)
        {
            return uiObjs.Select(MapToRoleAdminResult);
        }

        public static AddRoleCommand MapToAddRoleCommand(this RoleUiObj uiObj)
        {
            return new AddRoleCommand
            {
                Code = uiObj.Code,
                Description = uiObj.Description,
                AuthorizationIds = uiObj.AuthorizationIds,
                TenantId = null
            };
        }

        public static UpdateRoleCommand MapToUpdateRoleCommand(this RoleUiObj uiObj)
        {
            return new UpdateRoleCommand
            {
                Version = uiObj.Version,
                Code = uiObj.Code,
                Description = uiObj.Description,
                AuthorizationIds = uiObj.AuthorizationIds,
                TenantId = null
            };
        }
    }
}
