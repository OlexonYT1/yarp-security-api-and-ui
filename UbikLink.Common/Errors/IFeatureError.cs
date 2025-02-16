namespace UbikLink.Common.Errors
{
    public interface IFeatureError
    {
        public string Details { get; }
        public FeatureErrorType ErrorType { get; }
        public CustomError[] CustomErrors { get; }
    }
    public enum FeatureErrorType
    {
        NotFound = 404,
        BadParams = 400,
        Conflict = 409,
        NotAuthorized = 403,
        NotAuthentified = 401,
        ServerError = 500
    }
    public record CustomError
    {
        public string ErrorFriendlyMessage { get; init; } = default!;
        public string ErrorCode { get; init; } = default!;
        public IDictionary<string, string>? FieldsValuesInError { get; init; }
        public IDictionary<string, string>? DetailedMsgs { get; init; }
    }
}
