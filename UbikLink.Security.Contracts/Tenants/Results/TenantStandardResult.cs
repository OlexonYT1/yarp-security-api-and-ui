namespace UbikLink.Security.Contracts.Tenants.Results
{
    public record TenantStandardResult
    {
        public Guid Id { get; init; }
        public required Guid SubscriptionId { get; init; }
        public required string Label { get; init; }
        public bool IsActivated { get; init; } = true;
        public Guid Version { get; init; }
    }
}
