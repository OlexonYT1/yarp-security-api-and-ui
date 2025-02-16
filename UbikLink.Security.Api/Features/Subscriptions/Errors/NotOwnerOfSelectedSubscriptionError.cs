using UbikLink.Common.Errors;

namespace UbikLink.Security.Api.Features.Subscriptions.Errors
{
    public record NotOwnerOfSelectedSubscriptionError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public NotOwnerOfSelectedSubscriptionError(Guid userId)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"NOT_OWNER_OF_SELECTED_SUBSCRITION",
                ErrorFriendlyMessage = $"This user {userId} is not linked as the owner of the selected subscription.",
                FieldsValuesInError = new Dictionary<string, string>
                {
                    { "userId", userId.ToString() },
                }
            }];
        }
    }
}
