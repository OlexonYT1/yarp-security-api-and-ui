using UbikLink.Common.Errors;

namespace UbikLink.Security.Api.Features.Subscriptions.Errors
{
    public record TenantSubscriptionIsNotTheSameAsSelectedSubscriptionError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public TenantSubscriptionIsNotTheSameAsSelectedSubscriptionError(Guid tenantSubId, Guid selectedSubId)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"TENANT_SUB_IS_NOT_IN_SELECTED_SUBSCRIPTION",
                ErrorFriendlyMessage = $"This tenant subscription is not in the subscription you are working on.",
                FieldsValuesInError = new Dictionary<string, string>
                {
                    { "tenant subscription id", tenantSubId.ToString() },
                    { "selected subscription id", selectedSubId.ToString() }
                }
            }];
        }
    }
}
