using UbikLink.Common.Errors;

namespace UbikLink.Security.Api.Features.Users.Errors
{
    public record UserBadTenantLinkError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public UserBadTenantLinkError(Guid userId, Guid tenantId)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"USER_BAD_TENANT_LINK",
                ErrorFriendlyMessage = $"This user {userId} is not correctly linked to this tenant {tenantId}/subscription.",
                FieldsValuesInError = new Dictionary<string, string>
                {
                    { "userId", userId.ToString() },
                    { "tenantId", tenantId.ToString() }
                }
            }];
        }
    }
}
