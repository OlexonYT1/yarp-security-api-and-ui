using UbikLink.Common.Errors;

namespace UbikLink.Security.Api.Features.Users.Errors
{
    public record UserActivationError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public UserActivationError(Guid userId, string activationCode)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"USER_CANNOT_ACTIVATE",
                ErrorFriendlyMessage = $"Bad activation code provided",
                FieldsValuesInError = new Dictionary<string, string>
                {
                    { "userId", userId.ToString() },
                    { "activationCode", activationCode }
                }
            }];
        }
    }
}
