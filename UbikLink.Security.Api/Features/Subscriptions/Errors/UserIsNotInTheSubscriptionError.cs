using UbikLink.Common.Errors;

namespace UbikLink.Security.Api.Features.Subscriptions.Errors
{
    public record UserIsNotInTheSubscriptionError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public UserIsNotInTheSubscriptionError(Guid userId)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"USER_ID_NOT_VALID_FOR_THIS_SUBSCRIPTION",
                ErrorFriendlyMessage = $"This user {userId} is not linked to the selected subscription.",
                FieldsValuesInError = new Dictionary<string, string>
                {
                    { "userId", userId.ToString() },
                }
            }];
        }
    }
}
