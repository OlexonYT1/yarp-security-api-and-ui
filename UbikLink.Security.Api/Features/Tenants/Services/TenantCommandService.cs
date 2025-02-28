using Dapper;
using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Net;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.ErrorsShared;
using UbikLink.Security.Api.Features.Subscriptions.Errors;
using UbikLink.Security.Api.Features.Tenants.Services.Poco;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Users.Events;

namespace UbikLink.Security.Api.Features.Tenants.Services
{
    public class TenantCommandService(SecurityDbContext ctx, IPublishEndpoint publishEndpoint)
    {
        private readonly SecurityDbContext _ctx = ctx;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

        public async Task<Either<IFeatureError, TenantModel>> AddTenantInDbAsync(TenantModel tenant)
        {
            await _ctx.Tenants.AddAsync(tenant);
            _ctx.SetAuditAndSpecialFields();
            await _ctx.SaveChangesAsync();

            return tenant;
        }

        public async Task<Either<IFeatureError, TenantModel>> MapInDbContextAsync
            (TenantModel current, TenantModel forUpdate)
        {
            current = forUpdate.MapToTenant(current);
            await Task.CompletedTask;
            return current;
        }

        public async Task<Either<IFeatureError, TenantModel>> UpdateTenantInDbAsync(TenantModel current)
        {
            _ctx.Entry(current).State = EntityState.Modified;

            _ctx.SetAuditAndSpecialFields();
            await _ctx.SaveChangesAsync();

            return current;
        }

        public async Task<Either<IFeatureError, TenantModel>> GetTenantByIdAsync(Guid tenantId)
        {
            var tenant = await _ctx.Tenants.FindAsync(tenantId);

            return tenant == null
                ? new ResourceNotFoundError("Tenant", new Dictionary<string, string>
                    {
                        { "Id", tenantId.ToString() }
                    })
                : tenant;
        }

        public async Task<Either<IFeatureError, (TenantModel, SubscriptionModel)>> ValidateIfSubscriptionExistsAsync(TenantModel tenant)
        {
            var subscription = await _ctx.Subscriptions.FindAsync(tenant.SubscriptionId);

            return subscription == null
                ? new SubResourceNotFoundError("Tenant", "Subscription", new Dictionary<string, string>()
                    {
                        {"SubscriptionId", tenant.SubscriptionId.ToString()}
                    })
                : (tenant, subscription);
        }

        public async Task<Either<IFeatureError, TenantModel>> ValidateTenantLimitForSubscriptionAsync(TenantModel tenant,
            SubscriptionModel subscription,
            bool forUpdate)
        {
            if (!tenant.IsActivated)
                return tenant;

            var tenantCount = await _ctx.Tenants.CountAsync(t => t.SubscriptionId == subscription.Id
                                                            && t.IsActivated == true);
            if (forUpdate)
                tenantCount--;

            return tenantCount >= subscription.MaxTenants
                ? new MaxTenantsLimitForSubscriptionError(subscription.Id, subscription.MaxTenants)
                : tenant;
        }

        public async Task<Either<IFeatureError, (TenantModel Tenant, UserWithLinkedRoles User)>> 
            GetTenantUserWithRolesAsync(TenantModel tenant
            , Guid userId)
        {
            var con = _ctx.Database.GetDbConnection();
            var sql = $"""
                       SELECT u.*
                       FROM users u
                       INNER JOIN tenants_users tu ON u.id = tu.user_id
                       WHERE tu.tenant_id = @tenantId
                       AND u.id = @userId;
                       SELECT tu.user_id, tur.role_id
                       FROM tenants_users_roles tur
                       INNER JOIN tenants_users tu ON tu.id = tur.tenant_user_id
                       INNER JOIN roles r ON r.id = tur.role_id
                       WHERE tu.tenant_id = @tenantId
                       AND tu.user_id = @userId;
                       """;

            var result = new UserWithLinkedRoles();

            using (var multi = await con.QueryMultipleAsync(sql, new { tenantId = tenant.Id, userId }))
            {
                result = (await multi.ReadSingleOrDefaultAsync<UserWithLinkedRoles>());

                if (result == null)
                    return new ResourceNotFoundError("User", new Dictionary<string, string>()
                    {
                        {"Id", userId.ToString()}
                    });

                var roleLink = (await multi.ReadAsync<(Guid UserId, Guid RoleId)>())
                    .GroupBy(x => x.UserId)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.RoleId).ToList());

                if (roleLink.TryGetValue(result.Id, out var ids))
                {
                    result.LinkedRoleIds.AddRange(ids);
                }
            }

            if (con.State == System.Data.ConnectionState.Open)
                await con.CloseAsync();

            return (tenant,result);
        }

        public async Task<Either<IFeatureError, (Guid TenantUserId,
            TenantModel tenant,
            UserWithLinkedRoles user,
            List<Guid> RoleIdsForInsert,
            List<Guid> RoleIdsForDel)>>
        PrepareRoleIdsForAttachAndDetachFromUserUpdAsync(TenantModel tenant, UserWithLinkedRoles user, List<Guid> roleIds)
        {
            var roleIdsToAttach = new List<Guid>();
            var roleIdsToDetach = new List<Guid>();

            //Get tenant user id
            var tenantUser = await _ctx.TenantsUsers
                .Where(tu => tu.TenantId == tenant.Id && tu.UserId == user.Id)
                .SingleOrDefaultAsync();

            if (tenantUser == null)
                return new SubResourceNotFoundError("Tenant", "User", new Dictionary<string, string>()
                    {
                        {"TenantId", tenant.Id.ToString() },
                        {"UserId", user.Id.ToString() }
                    });

            //Get all possible roles in the tenant
            var possibleTenantRoleIds = await _ctx.Roles
                .Where(r => r.TenantId == tenant.Id || r.TenantId == null)
                .Select(r => r.Id)
                .ToListAsync();

            //Prepare roles for attach
            foreach (var roleId in roleIds)
            {
                if (!possibleTenantRoleIds.Contains(roleId))
                    return new SubResourceNotFoundError("TenantUser", "Role", new Dictionary<string, string>()
                    {
                        {"Id", roleId.ToString() }
                    });

                //Attach
                if (!user.LinkedRoleIds.Contains(roleId))
                    roleIdsToAttach.Add(roleId);
            }

            //Prepare role for detach
            roleIdsToDetach.AddRange(user.LinkedRoleIds.Except(roleIds));

            return (tenantUser.Id, tenant, user, roleIdsToAttach, roleIdsToDetach);
        }

        public async Task<Either<IFeatureError, bool>> UpdateTenantUserRole(TenantModel tenant, 
            UserWithLinkedRoles user,
            Guid tenantUserId,
            List<Guid> rolesIdsToAttach,
            List<Guid> rolesIdsToDetach)
        {
            var strategy = _ctx.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _ctx.Database.BeginTransactionAsync();
                await _ctx.TenantsUsersRoles
                .Where(tur => rolesIdsToDetach.Contains(tur.RoleId) && tur.TenantUserId == tenantUserId)
                .ExecuteDeleteAsync();

                foreach (var roleId in rolesIdsToAttach)
                {
                    _ctx.TenantsUsersRoles.Add(new TenantUserRoleModel
                    {
                        Id = NewId.NextGuid(),
                        TenantUserId = tenantUserId,
                        RoleId = roleId
                    });
                }

                _ctx.SetAuditAndSpecialFields();

                await _publishEndpoint.Publish(new CleanCacheForUserRequestSent
                {
                    TenantId = tenant.Id,
                    UserId = user.Id,
                    AuthId = user.AuthId
                });

                await _ctx.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            });
        }
    }
}
