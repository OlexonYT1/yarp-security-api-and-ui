using UbikLink.Security.Api.Features.Users.Commands.OnboardMeSimple;
using UbikLink.Security.Api.Features.Users.Commands.RegisterUser;
using UbikLink.Security.Api.Features.Users.Commands.UpdateUserSettingsMe;
using UbikLink.Security.Api.Features.Users.Queries.GetUserForProxy;
using UbikLink.Security.Api.Features.Users.Queries.GetUserMe;
using UbikLink.Security.Api.Features.Users.Services;

namespace UbikLink.Security.Api.Features.Users.Extensions
{
    public static class UserFeaturesRegistration
    {
        public static void AddUserFeatures(this IServiceCollection services)
        {
            services.AddScoped<GetUserForProxyHandler>();
            services.AddScoped<GetUserMeHandler>();

            services.AddScoped<UpdateUserSettingsMeHandler>();
            services.AddScoped<UpdateUserSettingsMeValidator>();

            services.AddScoped<UserQueryService>();
            services.AddScoped<UserCommandService>();

            services.AddScoped<RegisterUserHandler>();
            services.AddScoped<RegisterUserValidator>();

            services.AddScoped<OnboardMeSimpleHandler>();
        }
    }
}
