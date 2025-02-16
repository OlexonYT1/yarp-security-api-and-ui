using UbikLink.Common.Errors;

namespace UbikLink.Security.Api.Features.ErrorsShared
{
    public record MaxTenantsLimitForSubscriptionError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public MaxTenantsLimitForSubscriptionError(Guid subscriptionId, int maxTenantLimit)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"MAX_TENANTS_LIMIT_FOR_SUBSCRIPTION",
                ErrorFriendlyMessage = $"Subscription {subscriptionId}, is limited to {maxTenantLimit} tenant(s)",
                FieldsValuesInError = null
            }];
        }
    }
}
