using UbikLink.Common.Db;

namespace UbikLink.Security.Api.Data.Models
{
    //TODO: this security micro service is not the source of truth for subscription data (keep here only the data that impacts security etc)
    public class SubscriptionModel : IConcurrencyCheckEntity
    {
        public Guid Id { get; set; }
        public Guid Version { get; set; }
        public required string Label { get; set; }
        public required string PlanName { get; set; }
        public bool IsActive { get; set; } = true;
        public int MaxUsers { get; set; } = 1;
        public int MaxTenants { get; set; } = 1;
    }
}
