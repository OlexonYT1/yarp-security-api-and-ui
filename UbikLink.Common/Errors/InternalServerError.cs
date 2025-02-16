namespace UbikLink.Common.Errors
{
    public record InternalServerError : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public InternalServerError(IDictionary<string,string> labelsMessages)
        {
            ErrorType = FeatureErrorType.ServerError;
            CustomErrors = [ new()
            {
                ErrorCode = $"SERVER_ERROR",
                ErrorFriendlyMessage = $"Unexpected error.",
                DetailedMsgs = labelsMessages
            }];
        }
    }
}
