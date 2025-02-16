using MassTransit;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Roles.Services.Poco;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Authorizations.Results;
using UbikLink.Security.Contracts.Roles.Commands;
using UbikLink.Security.Contracts.Roles.Results;

namespace UbikLink.Security.Api.Mappers
{
    public static class RoleMappers
    {
        public static RoleAdminResult MapToRolesAdminResult(this RoleWithAuthorizationIds role)
        {
            return new RoleAdminResult
            {
                Id = role.Id,
                Code = role.Code,
                Description = role.Description,
                AuthorizationIds = role.AuthorizationIds,
                Version = role.Version
            };
        }

        public static IEnumerable<RoleAdminResult> MapToRolesAdminResults(this IEnumerable<RoleWithAuthorizationIds> roles)
        {
            return roles.Select(MapToRolesAdminResult);
        }

        public static (RoleModel Role, List<Guid> AuthorizationIds) MapToRoleWithAuthIds(this AddRoleCommand entity)
        {
            var role = new RoleModel
            {
                Id = NewId.NextGuid(),
                Code = entity.Code,
                Description = entity.Description,
                TenantId = entity.TenantId,
            };

            var authorizationIds = entity.AuthorizationIds;

            return (role, authorizationIds);
        }

        public static RoleModel MapToRole(this UpdateRoleCommand entity, Guid currentId)
        {
            return new RoleModel
            {
                Id = currentId,
                Code = entity.Code,
                Description = entity.Description,
                Version = entity.Version,
                TenantId = entity.TenantId
            };
        }

        public static RoleModel MapToRole(this RoleModel forUpd, RoleModel model)
        {
            model.Id = forUpd.Id;
            model.Code = forUpd.Code;
            model.Description = forUpd.Description;
            model.Version = forUpd.Version;
            model.TenantId = forUpd.TenantId;

            return model;
        }
        public static RoleLightResult MapToRoleLightResult(this RoleModel current)
        {
            return new RoleLightResult
            {
                Id = current.Id,
                Code = current.Code
            };
        }

        public static IEnumerable<RoleLightResult> MapToRoleLightResults(this IEnumerable<RoleModel> current)
        {
            return current.Select(MapToRoleLightResult);
        }


    }
}
