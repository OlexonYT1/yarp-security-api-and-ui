namespace UbikLink.Security.Api.Features.Subscriptions.Services.Poco
{
    public class TenantWithLinkedUsers
    {
        public Guid Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public required string Label { get; set; }
        public bool IsActivated { get; set; } = true;
        public List<Guid> LinkedUsers { get; set; } = [];
        public Guid Version { get; set; }
    }
}
