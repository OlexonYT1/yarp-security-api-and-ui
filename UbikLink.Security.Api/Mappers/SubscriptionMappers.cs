using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Contracts.Subscriptions.Results;

namespace UbikLink.Security.Api.Mappers
{
    public static class SubscriptionMappers
    {
        public static SubscriptionStandardResult MapToSubscriptionStandardResult(this SubscriptionModel model)
        {
            return new SubscriptionStandardResult
            {
                Id = model.Id,
                Label = model.Label,
                IsActive = model.IsActive,
                Version = model.Version
            };
        }

        public static IEnumerable<SubscriptionStandardResult> MapToSubscriptionStandardResults(this IEnumerable<SubscriptionModel> models)
        {
            return models.Select(model => model.MapToSubscriptionStandardResult());
        }

        public static SubscriptionOwnerResult MapToSubscriptionOwnerResult(this SubscriptionModel model)
        {
            return new SubscriptionOwnerResult
            {
                Id = model.Id,
                Label = model.Label,
                PlanName = model.PlanName,
                IsActive = model.IsActive,
                MaxTenants = model.MaxTenants,
                MaxUsers = model.MaxUsers,
                Version = model.Version
            };
        }

        public static IEnumerable<SubscriptionOwnerResult> MapToSubscriptionOwnerResults(this IEnumerable<SubscriptionModel> models)
        {
            return models.Select(model => model.MapToSubscriptionOwnerResult());
        }
    }
}
