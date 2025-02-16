namespace UbikLink.Common.Errors
{
    public class UpdateDbConcurrencyException : Exception, IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public CustomError[] CustomErrors { get; init; }
        public string Details => CustomErrors[0].ErrorFriendlyMessage;

        public UpdateDbConcurrencyException()
        {
            ErrorType = FeatureErrorType.Conflict;
           
            CustomErrors = [ new CustomError()
            {
                ErrorCode = "DB_CONCURRENCY_CONFLICT",
                ErrorFriendlyMessage = "You don't have the last version of this ressource, refresh your data before updating.",
                DetailedMsgs = new Dictionary<string, string>()
                 {
                    { "Field in error", "Version" }
                 }
            }];
        }
    }
}
