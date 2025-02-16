using UbikLink.Security.Api.Features.Subscriptions.Commands.AddTenantInSubscriptionForSubOwner;
using UbikLink.Security.Api.Features.Subscriptions.Queries.GetAllSubscriptionsMe;
using UbikLink.Security.Api.Features.Subscriptions.Queries.GetSubscriptionForOwner;
using UbikLink.Security.Api.Features.Subscriptions.Queries.GetSubscriptionAllLinkedTenantsForOwner;
using UbikLink.Security.Api.Features.Subscriptions.Queries.GetSubscriptionAllLinkedUsersForOwner;
using UbikLink.Security.Api.Features.Subscriptions.Services;
using UbikLink.Security.Api.Features.Subscriptions.Queries.GetSubscriptionLinkedTenantForOwner;
using UbikLink.Security.Api.Features.Subscriptions.Commands.UpdateTenantInSubscriptionForSubOwner;
using UbikLink.Security.Api.Features.Subscriptions.Commands.DeleteTenantInSubscriptionForSubOwner;
using UbikLink.Security.Api.Features.Subscriptions.Commands.UpdateUserInSubscriptionForSubOwner;
using UbikLink.Security.Api.Features.Subscriptions.Queries.GetSubscriptionLinkedUserForOwner;

namespace UbikLink.Security.Api.Features.Subscriptions.Extensions
{
    public static class SubscriptionFeaturesRegistration
    {
        public static void AddSubscriptionFeatures(this IServiceCollection services)
        {
            services.AddScoped<SubscriptionQueryService>();
            services.AddScoped<SubscriptionCommandService>();

            services.AddScoped<GetAllSubscriptionsMeHandler>();
            services.AddScoped<GetSubscriptionForOwnerHandler>();
            services.AddScoped<GetSubscriptionAllLinkedTenantsForOwnerHandler>();
            services.AddScoped<GetSubscriptionLinkedTenantForOwnerHandler>();
            services.AddScoped<GetSubscriptionLinkedUserForOwnerHandler>();
            services.AddScoped<GetSubscriptionAllLinkedUsersForOwnerHandler>();

            services.AddScoped<AddTenantInSubscriptionForSubOwnerHandler>();
            services.AddScoped<AddTenantInSubscriptionForSubOwnerValidator>();

            services.AddScoped<UpdateTenantInSubscriptionForSubOwnerHandler>();
            services.AddScoped<UpdateTenantInSubscriptionForSubOwnerValidator>();
            services.AddScoped<UpdateUserInSubscriptionForSubOwnerHandler>();
            services.AddScoped<UpdateUserInSubscriptionForSubOwnerValidator>();

            services.AddScoped<DeleteTenantInSubscriptionForSubOwnerHandler>();
        }
    }
}
