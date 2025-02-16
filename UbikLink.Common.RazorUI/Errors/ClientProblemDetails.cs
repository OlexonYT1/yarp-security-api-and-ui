using System.Text.Json.Serialization;

namespace UbikLink.Common.RazorUI.Errors
{
    public record ClientProblemDetails
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = default!;

        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;

        [JsonPropertyName("status")]
        public int Status { get; set; } = default!;

        [JsonPropertyName("detail")]
        public string Detail { get; set; } = default!;

        [JsonPropertyName("instance")]
        public string Instance { get; set; } = default!;

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; } = default!;

        [JsonPropertyName("traceId")]
        public string TraceId { get; set; } = default!;

        [JsonPropertyName("extensions")]
        public Dictionary<string, object?> Extensions { get; set; } = default!;

        [JsonPropertyName("errors")]
        public List<CustomError>? Errors { get; set; }

        [JsonPropertyName("validationErrors")]
        public Dictionary<string, string[]>? ValidationErrors { get; set; }
    }

    public record CustomError
    {
        [JsonPropertyName("errorFriendlyMessage")]
        public string ErrorFriendlyMessage { get; init; } = default!;

        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; init; } = default!;

        [JsonPropertyName("fieldsValuesInError")]
        public IDictionary<string, string>? FieldsValuesInError { get; init; }

        [JsonPropertyName("detailedMsgs")]
        public IDictionary<string, string>? DetailedMsgs { get; init; }
    }
}
