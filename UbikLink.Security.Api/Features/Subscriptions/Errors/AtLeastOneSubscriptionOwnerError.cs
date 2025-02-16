using UbikLink.Common.Errors;

namespace UbikLink.Security.Api.Features.Subscriptions.Errors
{
    public record AtLeastOneSubscriptionOwnerError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public AtLeastOneSubscriptionOwnerError(Guid userId)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"AT_LEAST_ONE_ACTIVATED_SUBSCRIPTION_OWNER",
                ErrorFriendlyMessage = $"This user {userId} cannot be disactivated or become a normal user. One sub owner needs to remain configured.",
                FieldsValuesInError = new Dictionary<string, string>
                {
                    { "userId", userId.ToString() },
                }
            }];
        }
    }
}
