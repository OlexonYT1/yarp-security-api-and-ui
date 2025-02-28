using UbikLink.Common.Errors;

namespace UbikLink.Security.Api.Features.Users.Errors
{
    public record UserOnBoardingError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public UserOnBoardingError(Guid userId)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"USER_CANNOT_ONBOARD",
                ErrorFriendlyMessage = $"Problem to onboard the user.",
                FieldsValuesInError = new Dictionary<string, string>
                {
                    { "userId", userId.ToString() },
                }
            }];
        }
    }
}
