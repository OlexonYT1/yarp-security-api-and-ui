using UbikLink.Security.Api.Features.Roles.Commands.AddRoleForAdmin;
using UbikLink.Security.Api.Features.Roles.Commands.BatchDeleteRoleForAdmin;
using UbikLink.Security.Api.Features.Roles.Commands.DeleteRoleForAdmin;
using UbikLink.Security.Api.Features.Roles.Commands.UpdateRoleForAdmin;
using UbikLink.Security.Api.Features.Roles.Queries.GetAllRolesForAdmin;
using UbikLink.Security.Api.Features.Roles.Queries.GetRoleForAdmin;
using UbikLink.Security.Api.Features.Roles.Services;

namespace UbikLink.Security.Api.Features.Roles.Extensions
{
    public static class RoleFeaturesRegistration
    {
        public static void AddRoleFeatures(this IServiceCollection services)
        {
            services.AddScoped<RoleQueryService>();
            services.AddScoped<RoleCommandService>();

            services.AddScoped<AddRoleForAdminValidator>();
            services.AddScoped<AddRoleForAdminHandler>();

            services.AddScoped<UpdateRoleForAdminValidator>();
            services.AddScoped<UpdateRoleForAdminHandler>();

            services.AddScoped<DeleteRoleForAdminHandler>();

            services.AddScoped<BatchDeleteRoleForAdminHandler>();

            services.AddScoped<GetRoleForAdminHandler>();
            services.AddScoped<GetAllRolesForAdminHandler>();
        }
    }
}
