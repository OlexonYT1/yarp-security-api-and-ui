namespace UbikLink.Common.Errors
{
    public record ResourceNotFoundError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public ResourceNotFoundError(string resourceName, IDictionary<string, string> fieldsValues)
        {

            ErrorType = FeatureErrorType.NotFound;
            CustomErrors = [ new()
            {
                ErrorCode = $"{resourceName.ToUpper()}_NOT_FOUND",
                ErrorFriendlyMessage = $"The {resourceName} is not found.",
                FieldsValuesInError = fieldsValues
            }];
        }
    }
}
