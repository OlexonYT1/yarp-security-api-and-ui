namespace UbikLink.Security.Contracts.Tenants.Commands
{
    public record AddTenantCommand
    {
        public required string Label { get; init; }
        public required Guid SubscriptionId { get; init; }
    }
}
