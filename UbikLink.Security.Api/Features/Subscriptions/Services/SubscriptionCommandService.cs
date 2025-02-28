using LanguageExt;
using MassTransit;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.ErrorsShared;
using UbikLink.Security.Api.Features.Subscriptions.Errors;
using UbikLink.Security.Api.Features.Subscriptions.Services.Poco;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Tenants.Events;
using UbikLink.Security.Contracts.Users.Events;

namespace UbikLink.Security.Api.Features.Subscriptions.Services
{
    public class SubscriptionCommandService(SecurityDbContext ctx, IPublishEndpoint publishEndpoint)
    {
        private readonly SecurityDbContext _ctx = ctx;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

        public async Task<Either<IFeatureError, TenantWithLinkedUsers>> AddTenantWithLinkedUsersInDbAsync(TenantModel tenant, List<Guid> userIds)
        {
            await _ctx.Tenants.AddAsync(tenant);
            await _ctx.TenantsUsers.AddRangeAsync(userIds.Select(userId => new TenantUserModel()
            {
                Id = NewId.NextGuid(),
                TenantId = tenant.Id,
                UserId = userId,
            }));

            _ctx.SetAuditAndSpecialFields();

            await _ctx.SaveChangesAsync();

            return new TenantWithLinkedUsers()
            {
                Id = tenant.Id,
                SubscriptionId = tenant.SubscriptionId,
                Label = tenant.Label,
                IsActivated = tenant.IsActivated,
                LinkedUsers = userIds,
                Version = tenant.Version,
            };
        }

        public async Task<Either<IFeatureError, (UserModel User, SubscriptionUserModel SubUser)>>
            UpdateSubscriptionUserInfoInDbAsync(UserModel user, SubscriptionUserModel subUser)
        {
            _ctx.Entry(user).State = EntityState.Modified;
            _ctx.Entry(subUser).State = EntityState.Modified;
            _ctx.SetAuditAndSpecialFields();

            await _publishEndpoint.Publish(new CleanCacheForUserRequestSent()
            {
                UserId = user.Id,
                TenantId = user.SelectedTenantId,
                AuthId = user.AuthId
            });

            await _ctx.SaveChangesAsync();
            return (user, subUser);
        }

        public async Task<Either<IFeatureError, TenantWithLinkedUsers>> UpdateTenantWithLinkedUsersInDbAsync(TenantModel tenant,
            List<Guid> attachUserIds,
            List<Guid> detachUserIds)
        {
            var strategy = _ctx.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using (var transaction = await _ctx.Database.BeginTransactionAsync())
                {
                    _ctx.Entry(tenant).State = EntityState.Modified;

                    await _ctx.TenantsUsers
                    .Where(tu => detachUserIds.Contains(tu.UserId) && tu.TenantId == tenant.Id)
                    .ExecuteDeleteAsync();

                    await _ctx.TenantsUsers.AddRangeAsync(attachUserIds.Select(userId => new TenantUserModel()
                    {
                        Id = NewId.NextGuid(),
                        TenantId = tenant.Id,
                        UserId = userId,
                    }));

                    _ctx.SetAuditAndSpecialFields();

                    //publish info for cache cleaning
                    await _publishEndpoint.Publish(new CleanCacheTenantUpdated()
                    {
                        TenantId = tenant.Id
                    });

                    await _ctx.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                var userIds = await _ctx.TenantsUsers
                    .Where(tu => tu.TenantId == tenant.Id)
                    .Select(tu => tu.UserId)
                    .ToListAsync();

                return new TenantWithLinkedUsers()
                {
                    Id = tenant.Id,
                    SubscriptionId = tenant.SubscriptionId,
                    Label = tenant.Label,
                    IsActivated = tenant.IsActivated,
                    LinkedUsers = userIds,
                    Version = tenant.Version,
                };
            });
        }

        public async Task<Either<IFeatureError, TenantModel>> MapTenantInDbContextAsync
            (TenantModel current, TenantModel forUpdate)
        {
            current = forUpdate.MapToTenant(current);
            await Task.CompletedTask;
            return current;
        }

        public async Task<Either<IFeatureError, (UserModel User,SubscriptionUserModel SubUser, SubscriptionModel Subscription)>> 
            MapUserInfoInDbContextAsync(UserModel current,
                SubscriptionUserModel currentSubUser,
                SubscriptionModel subscription,
                UserForUpd forUpdate)
        {
            (current,currentSubUser) = forUpdate.MapToUserAndSubUser(current,currentSubUser);
            await Task.CompletedTask;
            return (current, currentSubUser, subscription);
        }

        public async Task<Either<IFeatureError, (TenantModel Tenant, SubscriptionModel Subscription)>>
            GetTenantInSubscriptionAsyc(Guid tenantId,
            Guid requestedSubscriptionId,
            SubscriptionModel selectedSubscription)
        {
            if (requestedSubscriptionId != selectedSubscription.Id)
                return new ResourceNotFoundError("Tenant", new Dictionary<string, string>()
                    {
                        { "tenantId", tenantId.ToString() },
                        { "subscriptionId", requestedSubscriptionId.ToString() }
                    });

            var tenant = await _ctx.Tenants
                .SingleOrDefaultAsync(t => t.Id == tenantId
                && t.SubscriptionId == requestedSubscriptionId);

            return tenant == null
                ? new ResourceNotFoundError("Tenant", new Dictionary<string, string>()
                    {
                        { "tenantId", tenantId.ToString() },
                        { "subscriptionId", requestedSubscriptionId.ToString() }
                    })
                : (tenant, selectedSubscription);
        }

        public async Task<Either<IFeatureError,(UserModel User, 
            SubscriptionUserModel SubUser, 
            SubscriptionModel Subscription)>>
            GetUserInfoInSubscription(Guid userId, 
                SubscriptionModel selectedSubscription)
        {
            var user = await _ctx.Users
                .FromSql($"""
                         SELECT u.* 
                         FROM users u 
                         INNER JOIN subscriptions_users su ON u.Id = su.user_id 
                         WHERE su.user_id = {userId}
                         AND su.subscription_id = {selectedSubscription.Id}
                         """).ToListAsync();

            if (user == null || user.Count != 1)
                return new ResourceNotFoundError("User", new Dictionary<string, string>()
                    {
                        { "userId", userId.ToString() },
                        { "subscriptionId", selectedSubscription.Id.ToString() }
                    });

            var subUser = await _ctx.SubscribtionsUsers.Where(x=>x.UserId == userId 
                && x.SubscriptionId == selectedSubscription.Id)
                .SingleAsync();

            return (user.First(), subUser, selectedSubscription);
        }

        public async Task<Either<IFeatureError, (TenantModel Tenant, SubscriptionModel Subscription)>>
            GetTenantInSubscriptionAsyc(Guid tenantId,
            SubscriptionModel selectedSubscription)
        {
            var tenant = await _ctx.Tenants
                .SingleOrDefaultAsync(t => t.Id == tenantId
                && t.SubscriptionId == selectedSubscription.Id);

            return tenant == null
                ? new ResourceNotFoundError("Tenant", new Dictionary<string, string>()
                    {
                        { "tenantId", tenantId.ToString() },
                        { "subscriptionId", selectedSubscription.Id.ToString() }
                    })
                : (tenant, selectedSubscription);
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

        public async Task<Either<IFeatureError, (TenantModel Tenant, SubscriptionModel Subscription)>>
            ValidateIfTenantSubscriptionIsSameAsSelectedForOwnerAsync(TenantModel tenant,
            SubscriptionModel selectedSub)
        {
            if (tenant.SubscriptionId != selectedSub.Id)
                return new TenantSubscriptionIsNotTheSameAsSelectedSubscriptionError(tenant.Id, selectedSub.Id);

            await Task.CompletedTask;
            return (tenant, selectedSub);
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
            {
                var curTenant = await _ctx.Tenants.FindAsync(tenant.Id);
                if (curTenant?.IsActivated ?? false)
                    tenantCount--;
            }

            return tenantCount >= subscription.MaxTenants
                ? new MaxTenantsLimitForSubscriptionError(subscription.Id, subscription.MaxTenants)
                : tenant;
        }

        public async Task<Either<IFeatureError, (UserModel User, SubscriptionUserModel SubUser)>>
            ValidateUserInfoForSubscriptionAsync(UserModel user,
            SubscriptionUserModel subUser,
            SubscriptionModel subscription,
            bool forUpdate)
        {
            var subscriptionUsers = await _ctx.SubscribtionsUsers
                .Where(su => su.SubscriptionId == subscription.Id).ToListAsync();

            var currentOwners = subscriptionUsers.Where(x=>x.IsOwner && x.IsActivated).ToList();

            if(currentOwners.Count == 1 
                && subUser.Id == currentOwners.First().Id
                && (!subUser.IsOwner || !subUser.IsActivated))
                return new AtLeastOneSubscriptionOwnerError(subscription.Id);

            if (!subUser.IsActivated)
                return (user, subUser);

            var count = await _ctx.SubscribtionsUsers.CountAsync(t => t.SubscriptionId == subscription.Id
                                                            && t.IsActivated == true);
            if (forUpdate)
            {
                var curUser = await _ctx.SubscribtionsUsers.FindAsync(subUser.Id);
                if (curUser?.IsActivated ?? false)
                    count--;
            }

            return count >= subscription.MaxUsers
                ? new MaxUsersLimitForSubscriptionError(subscription.Id, subscription.MaxUsers)
                : (user,subUser);
        }

        /// <summary>
        /// Return an error if a userId is not valid. If success, it returns the userIds to be inserted and to deleted
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="userIds"></param>
        /// <param name="forUpdate"></param>
        /// <returns></returns>
        public async Task<Either<IFeatureError, (TenantModel Tenant,
            List<Guid> UserIdsForInsert,
            List<Guid> UserIdsForDel)>>
            PrepareUserIdsForAttachAndDetachFromTenantAsync(TenantModel tenant, List<Guid> userIds, bool forUpdate)
        {
            var userIdsToAttach = new List<Guid>();
            var userIdsToDetach = new List<Guid>();

            //Get all users in the subscription
            var subscriptionUserIds = await _ctx.SubscribtionsUsers
                .Where(su => su.SubscriptionId == tenant.SubscriptionId)
                .Select(su => su.UserId)
                .ToListAsync();

            //Get all users in the tenant
            var tenantUserIds = await _ctx.TenantsUsers
                .Where(tu => tu.TenantId == tenant.Id)
                .Select(tu => tu.UserId)
                .ToListAsync();

            //Prepare users for attach
            foreach (var userId in userIds)
            {
                if (!subscriptionUserIds.Contains(userId))
                    return new UserIsNotInTheSubscriptionError(userId);

                //Attach
                if (!tenantUserIds.Contains(userId))
                    userIdsToAttach.Add(userId);
            }

            //When adding a new tenant, we don't need to detach any user
            if (!forUpdate)
                return (tenant, userIdsToAttach, userIdsToDetach);

            //Prepare users for detach (when update)
            userIdsToDetach.AddRange(tenantUserIds.Except(userIds));

            return (tenant, userIdsToAttach, userIdsToDetach);
        }

        public async Task<Either<IFeatureError, Guid>> DeleteTenantInDbAsync(TenantModel tenant)
        {
            _ctx.Tenants.Remove(tenant);

            await _publishEndpoint.Publish(new CleanCacheTenantDeleted()
            {
                TenantId = tenant.Id
            });

            await _ctx.SaveChangesAsync();

            return tenant.Id;
        }
    }
}
