using UbikLink.Security.Api.Features.Tenants.Commands.AddTenantForAdmin;
using UbikLink.Security.Api.Features.Tenants.Commands.UpdateTenantForAdmin;
using UbikLink.Security.Api.Features.Tenants.Queries.GetAllTenantsForAdmin;
using UbikLink.Security.Api.Features.Tenants.Queries.GetAllTenantsMe;
using UbikLink.Security.Api.Features.Tenants.Queries.GetTenant;
using UbikLink.Security.Api.Features.Tenants.Queries.GetTenantForAdmin;
using UbikLink.Security.Api.Features.Tenants.Queries.GetTenantLinkedUser;
using UbikLink.Security.Api.Features.Tenants.Queries.GetAllTenantLinkedUsers;
using UbikLink.Security.Api.Features.Tenants.Services;
using UbikLink.Security.Api.Features.Users.Queries.GetUserForProxy;
using UbikLink.Security.Api.Features.Tenants.Commands.UpdateTenantLinkedUser;
using UbikLink.Security.Api.Features.Tenants.Queries.GetTenantRoles;

namespace UbikLink.Security.Api.Features.Tenants.Extensions
{
    public static class TenantFeaturesRegistration
    {
        public static void AddTenantFeatures(this IServiceCollection services)
        {
            services.AddScoped<TenantCommandService>();
            services.AddScoped<TenantQueryService>();
            services.AddScoped<AddTenantForAdminHandler>();
            services.AddScoped<AddTenantForAdminValidator>();
            services.AddScoped<UpdateTenantForAdminHandler>();
            services.AddScoped<UpdateTenantForAdminValidator>();
            services.AddScoped<GetAllTenantsForAdminHandler>();
            services.AddScoped<GetTenantForAdminHandler>();
            services.AddScoped<GetAllTenantsMeHandler>();
            services.AddScoped<GetTenantHandler>();
            services.AddScoped<GetAllTenantLinkedUsersHandler>();
            services.AddScoped<GetTenantLinkedUserHandler>();
            services.AddScoped<UpdateTenantLinkedUserHandler>();
            services.AddScoped<GetTenantRolesHandler>();
        }
    }
}
