using UbikLink.Common.Errors;

namespace UbikLink.Security.Api.Features.Users.Errors
{
    public record GetHubTokenError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public GetHubTokenError()
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"CANNOT_GET_HUB_TOKEN",
                ErrorFriendlyMessage = $"The connected user request is not valid to receive a hub token.",
                FieldsValuesInError = new Dictionary<string, string>
                {
                    { "userId", "connected user" },
                }
            }];
        }
    }
}
