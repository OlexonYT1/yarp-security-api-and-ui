using UbikLink.Security.Api.Features.Authorizations.Commands.AddAuthorizationForAdmin;
using UbikLink.Security.Api.Features.Authorizations.Commands.BatchDeleteAuthorizationForAdmin;
using UbikLink.Security.Api.Features.Authorizations.Commands.DeleteAuthorizationForAdmin;
using UbikLink.Security.Api.Features.Authorizations.Commands.UpdateAuthorizationForAdmin;
using UbikLink.Security.Api.Features.Authorizations.Queries.GetAllAuthorizationsForAdmin;
using UbikLink.Security.Api.Features.Authorizations.Queries.GetAuthorizationForAdmin;
using UbikLink.Security.Api.Features.Authorizations.Services;
using UbikLink.Security.Api.Features.Subscriptions.Queries.GetAllSubscriptionsMe;

namespace UbikLink.Security.Api.Features.Authorizations.Extensions
{
    public static class AuthorizationFeaturesRegistration
    {
        public static void AddAuthorizationFeatures(this IServiceCollection services)
        {
            services.AddScoped<AuthorizationCommandService>();
            services.AddScoped<AuthorizationQueryService>();

            services.AddScoped<GetAllAuthorizationsForAdminHandler>();
            services.AddScoped<GetAuthorizationForAdminHandler>();

            services.AddScoped<AddAuthorizationForAdminHandler>();
            services.AddScoped<AddAuthorizationForAdminValidator>();

            services.AddScoped<DeleteAuthorizationForAdminHandler>();
            services.AddScoped<BatchDeleteAuthorizationForAdminHandler>();

            services.AddScoped<UpdateAuthorizationForAdminHandler>();
            services.AddScoped<UpdateAuthorizationForAdminValidator>();
        }
    }
}
