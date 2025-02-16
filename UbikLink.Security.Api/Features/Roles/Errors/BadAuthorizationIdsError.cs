using UbikLink.Common.Errors;

namespace UbikLink.Security.Api.Features.Roles.Errors
{
    public record BadAuthorizationIdsError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public BadAuthorizationIdsError(IEnumerable<Guid> missingIds)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"AUTHORIZATION_BAD_IDS",
                ErrorFriendlyMessage = $"The ids are not present.",
                FieldsValuesInError = new Dictionary<string, string>
                {
                    { "Ids", string.Join(", ", missingIds) }
                }
            }];
        }
    }
}
