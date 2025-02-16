namespace UbikLink.Common.Errors
{
    public record ResourceAlreadyExistsError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public ResourceAlreadyExistsError(string resourceName, IDictionary<string,string> fieldsValues)
        {

            ErrorType = FeatureErrorType.Conflict;
            CustomErrors = [ new()
            {
                ErrorCode = $"{resourceName.ToUpper()}_ALREADY_EXISTS",
                ErrorFriendlyMessage = $"The {resourceName} already exists.",
                FieldsValuesInError = fieldsValues
            }];
        }
    }
}
