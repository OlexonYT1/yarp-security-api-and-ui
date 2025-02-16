namespace UbikLink.Security.Api.Features.Subscriptions.Services.Poco
{
    public class UserLinkedTenant
    {
        public Guid Id { get; set; }
        public string Label { get; set; } = default!;
        public Guid UserId { get; set; }
    }
}
