using MassTransit;
using Microsoft.EntityFrameworkCore;
using UbikLink.Common.Api;
using UbikLink.Common.Db;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Config;
using UbikLink.Security.Api.Data.Models;

namespace UbikLink.Security.Api.Data
{
    public class SecurityDbContext(DbContextOptions<SecurityDbContext> options) : DbContext(options)
    {
        public ICurrentUser CurrentUser { get; set; } = default!;
        public DbSet<TenantModel> Tenants { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<SubscriptionModel> Subscriptions { get; set; }
        public DbSet<AuthorizationModel> Authorizations { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<SubscriptionUserModel> SubscribtionsUsers { get; set; }
        public DbSet<TenantUserModel> TenantsUsers { get; set; }
        public DbSet<RoleAuthorizationModel> RolesAuthorizations { get; set; }
        public DbSet<TenantUserRoleModel> TenantsUsersRoles { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                var err = new UpdateDbConcurrencyException();

                throw err;
            }
        }

        public void SetAuditAndSpecialFields()
        {
            ChangeTracker.SetSpecialFields(CurrentUser);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();

            SetTenantId(modelBuilder);

            new TenantConfiguration().Configure(modelBuilder.Entity<TenantModel>());
            new UserConfiguration().Configure(modelBuilder.Entity<UserModel>());
            new SubscriptionConfiguration().Configure(modelBuilder.Entity<SubscriptionModel>());
            new RoleConfiguration().Configure(modelBuilder.Entity<RoleModel>());
            new AuthorizationConfiguration().Configure(modelBuilder.Entity<AuthorizationModel>());
            new SubscriptionUserConfiguration().Configure(modelBuilder.Entity<SubscriptionUserModel>());
            new TenantUserConfiguration().Configure(modelBuilder.Entity<TenantUserModel>());
            new RoleAuthorizationConfiguration().Configure(modelBuilder.Entity<RoleAuthorizationModel>());
            new TenantUserRoleConfiguration().Configure(modelBuilder.Entity<TenantUserRoleModel>());

            base.OnModelCreating(modelBuilder);
        }

        private void SetTenantId(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Account>()
            //    .HasQueryFilter(mt => mt.TenantId == _currentUser.TenantId);
        }
    }
}
