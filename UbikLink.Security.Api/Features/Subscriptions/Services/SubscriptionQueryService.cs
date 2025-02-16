using Dapper;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Subscriptions.Errors;
using UbikLink.Security.Api.Features.Subscriptions.Services.Poco;

namespace UbikLink.Security.Api.Features.Subscriptions.Services
{
    public class SubscriptionQueryService(SecurityDbContext ctx)
    {
        private readonly SecurityDbContext _ctx = ctx;

        public async Task<IEnumerable<SubscriptionModel>> GetAllSubscriptionsForUserAsync(Guid userId)
        {
            var subscriptions = await _ctx.Subscriptions
                .FromSql($"""
                         SELECT s.* 
                         FROM subscriptions s 
                         INNER JOIN subscriptions_users su ON s.Id = su.subscription_id 
                         WHERE su.user_id = {userId}
                         AND su.is_activated = true
                         """)
                .ToListAsync();

            return subscriptions;
        }

        public async Task<Either<IFeatureError, SubscriptionModel>> GetSelectedSubscriptionForOwnerAsync(Guid userId)
        {
            var result = await _ctx.Subscriptions
                .FromSql($"""
                         SELECT s.* 
                         FROM subscriptions s 
                         INNER JOIN subscriptions_users su ON s.Id = su.subscription_id 
                         INNER JOIN tenants t ON t.subscription_id = s.Id
                         INNER JOIN users u ON su.user_id = u.Id
                         WHERE su.user_id = {userId}
                         AND t.Id = u.selected_tenant_id
                         AND su.is_owner = true
                         AND su.is_activated = true
                         """)
                .SingleOrDefaultAsync();

            return result == null
                ? new NotOwnerOfSelectedSubscriptionError(userId)
                : result;
        }

        public async Task<Either<IFeatureError, TenantWithLinkedUsers>> GetSubscriptionLinkedTenantAsync(Guid subscriptionId, Guid tenantId)
        {
            var con = _ctx.Database.GetDbConnection();
            var sql = $"""
                       SELECT t.* 
                       FROM tenants t 
                       WHERE t.subscription_id = @subscriptionId 
                       AND t.id = @tenantId;
                       SELECT tu.user_id
                       FROM tenants_users tu
                       INNER JOIN tenants t2 ON tu.tenant_id = t2.id
                       WHERE t2.subscription_id = @subscriptionId
                       AND t2.id = @tenantId;
                       """;

            var result = new TenantWithLinkedUsers()
            {
                Label = string.Empty
            };

            using (var multi = await con.QueryMultipleAsync(sql, new { subscriptionId, tenantId }))
            {
                result = await multi.ReadFirstOrDefaultAsync<TenantWithLinkedUsers>();

                if (result == null)
                    return new ResourceNotFoundError("Tenant", new Dictionary<string, string>()
                    {
                        { "subscriptionId", subscriptionId.ToString() },
                        { "tenantId", tenantId.ToString() }
                     });

                var tenantUserIds = await multi.ReadAsync<Guid>();
                result.LinkedUsers.AddRange(tenantUserIds);
            }

            if (con.State == System.Data.ConnectionState.Open)
                await con.CloseAsync();

            return result;
        }

        public async Task<Either<IFeatureError, (UserModel User, SubscriptionUserModel SubInfo)>> 
            GetSubscriptionLinkedUserWithInfoAsync(Guid subscriptionId, Guid userId)
        {
            var con = _ctx.Database.GetDbConnection();
            var sql = $"""
                       SELECT u.*, su.*
                       FROM users u
                       INNER JOIN subscriptions_users su ON u.id = su.user_id
                       WHERE su.subscription_id = @subscriptionId
                       AND u.id = @userId;
                       """;

            var result = await con.QueryAsync<UserModel, SubscriptionUserModel, 
                (UserModel User, SubscriptionUserModel SubInfo)>(
                    sql,
                    (user, subInfo) => (user, subInfo),
                    new { subscriptionId, userId },
                    splitOn: "id"
            );

            if (result == null || result.Count() != 1)
                return new ResourceNotFoundError("User", new Dictionary<string, string>()
                {
                    { "subscriptionId", subscriptionId.ToString() },
                    { "userId", userId.ToString() }
                });

            var userAndSubUser = result.First();

            if (con.State == System.Data.ConnectionState.Open)
                await con.CloseAsync();

            return userAndSubUser;
        }

        public async Task<Either<IFeatureError, List<TenantWithLinkedUsers>>> GetSubscriptionAllLinkedTenantsAsync(Guid subscriptionId)
        {
            var con = _ctx.Database.GetDbConnection();

            var sql = $"""
                       SELECT t.*
                       FROM tenants t
                       WHERE t.subscription_id = @subscriptionId;
                       SELECT tu.tenant_id, tu.user_id
                       FROM tenants_users tu
                       INNER JOIN tenants t2 ON tu.tenant_id = t2.id
                       WHERE t2.subscription_id = @subscriptionId;
                       """;

            var result = new List<TenantWithLinkedUsers>();

            using (var multi = await con.QueryMultipleAsync(sql, new { subscriptionId }))
            {
                result = [.. (await multi.ReadAsync<TenantWithLinkedUsers>())];
                var tenantUserIds = await multi.ReadAsync<(Guid TenantId,Guid UserId)>();

                foreach (var tenant in result)
                {
                    tenant.LinkedUsers.AddRange(tenantUserIds
                        .Where(x => x.TenantId == tenant.Id)
                        .Select(x => x.UserId));
                }
            }

            if (con.State == System.Data.ConnectionState.Open)
                await con.CloseAsync();

            return result.ToList();
        }

        public async Task<Either<IFeatureError, List<UserWithLinkedTenants>>> GetSubscriptionAllLinkedUsersAsync(Guid subscriptionId)
        {
            var con = _ctx.Database.GetDbConnection();

            //First query to get all the user linked to a sub
            //Second query to get all the tenants linked to a user in a sub
            var sql = $"""
                       SELECT u.Id, 
                       u.firstname, 
                       u.lastname, 
                       u.email,
                       su.is_activated,
                       su.is_owner,
                       u.version
                       FROM users u 
                       INNER JOIN subscriptions_users su ON u.Id = su.user_id
                       WHERE su.subscription_id = @subscriptionId;
                       SELECT t.id,
                       t.label,
                       tu.user_id
                       FROM tenants t
                       INNER JOIN tenants_users tu ON t.id = tu.tenant_id
                       WHERE t.subscription_id = @subscriptionId;
                       """;

            var result = new List<UserWithLinkedTenants>();

            using (var multi = await con.QueryMultipleAsync(sql, new { subscriptionId }))
            {
                result = [.. (await multi.ReadAsync<UserWithLinkedTenants>())];
                var tenants = await multi.ReadAsync<UserLinkedTenant>();

                foreach (var user in result)
                {
                    user.LinkedTenants.AddRange(tenants.Where(t=>t.UserId == user.Id));
                }
            }

            //To be sure dapper is closing the connection (see if it can have side effect)
            //I think because I don't use tracking etc... the risk is minimal.
            if (con.State == System.Data.ConnectionState.Open)
                await con.CloseAsync();

            return result;
        }
    }
}
