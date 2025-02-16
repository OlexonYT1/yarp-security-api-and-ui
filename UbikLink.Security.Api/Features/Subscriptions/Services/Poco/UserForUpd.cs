namespace UbikLink.Security.Api.Features.Subscriptions.Services.Poco
{
    public record UserForUpd
    {
        public required string Firstname { get; init; }
        public required string Lastname { get; init; }
        public bool IsActivated { get; init; } = true;
        public bool IsSubscriptionOwner { get; init; } = true;
        public Guid Version { get; init; }
    }
}
