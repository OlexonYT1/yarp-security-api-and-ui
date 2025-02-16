namespace UbikLink.Security.UI.Client.Components.Subscription
{
    public class SubscriptionUiObj
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
        public required string Label { get; set; }
        public required string PlanName { get; set; }
        public List<TenantUiObj> Tenants { get; set; } = [];
        public IQueryable<UserUiObj> Users { get; set; } = default!;
        public int MaxUsers { get; set; } = 1;
        public int MaxTenants { get; set; } = 1;
        public Guid Version { get; set; }

        public bool CanEditOrDeleteTenant
        {
            get
            {
                return Tenants.Count(t => t.Selected) == 1;
            }
        }

        public bool CanTenantToggleActivation
        {
            get
            {
                var alreadyActivated = Tenants.Count(t => t.IsActivated);
                var selected = Tenants.Where(t => t.Selected);

                if (selected.Count() != 1)
                    return false;

                var tenant = selected.First();

                if (tenant.IsActivated)
                    return true;

                if(alreadyActivated < MaxTenants)
                    return true;

                return false;
            }
        }


        public bool CanAddTenant
        {
            get
            {
                return Tenants.Count(t => t.IsActivated) < MaxTenants;
            }
        }
    }
}
