namespace UbikLink.Common.Errors
{
    public class SubResourceNotFoundError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public SubResourceNotFoundError(string resourceName, string subResourceName, IDictionary<string, string> fieldsValues)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [ new()
            {
                ErrorCode = $"{subResourceName.ToUpper()}_FOR_{resourceName.ToUpper()}_NOT_FOUND",
                ErrorFriendlyMessage = $"The {subResourceName} is not found.",
                FieldsValuesInError = fieldsValues
            }];
        }
    }
}
