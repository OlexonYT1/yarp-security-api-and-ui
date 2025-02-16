using UbikLink.Common.Errors;

namespace UbikLink.Security.Api.Features.ErrorsShared
{
    public record MaxUsersLimitForSubscriptionError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public MaxUsersLimitForSubscriptionError(Guid subscriptionId, int maxUserLimit)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"MAX_USERS_LIMIT_FOR_SUBSCRIPTION",
                ErrorFriendlyMessage = $"Subscription {subscriptionId}, is limited to {maxUserLimit} user(s)",
                FieldsValuesInError = null
            }];
        }
    }
}
