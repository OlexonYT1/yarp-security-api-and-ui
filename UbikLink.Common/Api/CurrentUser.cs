namespace UbikLink.Common.Api
{
    public class CurrentUser : ICurrentUser
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public bool IsMegaAdmin { get; set; } = false;
        public Guid? TenantId { get; set; } = null;
    }
}
