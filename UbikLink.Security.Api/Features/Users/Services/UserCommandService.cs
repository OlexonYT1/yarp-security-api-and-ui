using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data;
using UbikLink.Security.Api.Features.Users.Errors;
using UbikLink.Security.Contracts.Users.Events;
using Dapper;
using UbikLink.Security.Api.Data.Models;

namespace UbikLink.Security.Api.Features.Users.Services
{
    public class UserCommandService(SecurityDbContext ctx, IPublishEndpoint publishEndpoint)
    {
        private readonly SecurityDbContext _ctx = ctx;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        private static readonly string _devActivationCode = "dev-activationcode";
        private static readonly string _tenantBaseRoleForOnBoarding = "TenantManager";

        public async Task<Either<IFeatureError, UserModel>> GetUserIfNotAlreadyOnBoardedAsync(Guid userId)
        {
            var user = await _ctx.Users.SingleOrDefaultAsync(u => u.Id == userId && u.IsEmailVerified == false);
            return user == null
                ? new ResourceNotFoundError("User", new Dictionary<string, string> { { "Id", userId.ToString() } })
                : user;
        }

        public async Task<Either<IFeatureError, UserModel>> SimpleOnboardingAsync(UserModel user)
        {
            var (User, Sub) = await AddNewSubscriptionAndAttachUserAsOwnerAsync(user);
            var userTenant = await AddTenantToSubscriptionAndAttachUserAsync(User, Sub);
            var result = await AddBaseRoleAsync(userTenant.User, userTenant.TenantUser);

            result.SelectedTenantId = userTenant.TenantUser.TenantId;
            _ctx.Users.Update(result);

            _ctx.SetAuditAndSpecialFields();

            await _publishEndpoint.Publish(new CleanCacheForUserRequestSent()
            {
                AuthId = result.AuthId,
                UserId = result.Id,
                TenantId = userTenant.TenantUser.TenantId,
            }, CancellationToken.None);

            await _ctx.SaveChangesAsync();

            return result;
        }

        public async Task<Either<IFeatureError, UserModel>> ActivateUserEmailInContext(UserModel user, bool isTrueActivation, string activationCode)
        {
            if (isTrueActivation && activationCode == user.ActivationCode)
            {
                user.IsEmailVerified = true;
                return user;
            }

            if (!isTrueActivation && activationCode == _devActivationCode)
            {
                user.IsEmailVerified = true;
                return user;
            }

            await Task.CompletedTask;
            return new UserActivationError(user.Id, activationCode);
        }

        private async Task<UserModel> AddBaseRoleAsync(UserModel user, TenantUserModel tenantUser)
        {
            var role = await _ctx.Roles.SingleAsync(r => r.Code == _tenantBaseRoleForOnBoarding);

            var tenantUserRole = new TenantUserRoleModel()
            {
                Id = NewId.NextGuid(),
                TenantUserId = tenantUser.Id,
                RoleId = role.Id,
            };

            await _ctx.TenantsUsersRoles.AddAsync(tenantUserRole);

            return user;
        }

        private async Task<(UserModel User, TenantUserModel TenantUser)> AddTenantToSubscriptionAndAttachUserAsync(UserModel user, SubscriptionModel sub)
        {
            var tenant = new TenantModel()
            {
                Id = NewId.NextGuid(),
                Label = $"Your tenant",
                SubscriptionId = sub.Id,
                IsActivated = true,
            };

            await _ctx.Tenants.AddAsync(tenant);

            var tenantUser = new TenantUserModel()
            {
                Id = NewId.NextGuid(),
                TenantId = tenant.Id,
                UserId = user.Id,
            };

            await _ctx.TenantsUsers.AddAsync(tenantUser);

            return (user, tenantUser);
        }

        private async Task<(UserModel User, SubscriptionModel Sub)> AddNewSubscriptionAndAttachUserAsOwnerAsync(UserModel user)
        {
            //Create new subscription
            var sub = new SubscriptionModel()
            {
                Id = NewId.NextGuid(),
                Label = $"{user.Firstname} {user.Lastname} subscription",
                IsActive = true,
                MaxTenants = 1,
                MaxUsers = 1,
                PlanName = "Free",
            };

            await _ctx.Subscriptions.AddAsync(sub);

            var subUser = new SubscriptionUserModel()
            {
                Id = NewId.NextGuid(),
                SubscriptionId = sub.Id,
                UserId = user.Id,
                IsActivated = true,
                IsOwner = true,
            };

            await _ctx.SubscribtionsUsers.AddAsync(subUser);

            return (user, sub);
        }

        public async Task<UserModel> AddUserInDbAsync(UserModel current)
        {
            await _ctx.Users.AddAsync(current);
            _ctx.SetAuditAndSpecialFields();
            await _ctx.SaveChangesAsync();

            return current;
        }

        public async Task<Either<IFeatureError, UserModel>> SaveAndPublishNewSelectedTenantForUser(UserModel currentUser, Guid tenantId)
        {
            currentUser.SelectedTenantId = tenantId;
            _ctx.Entry(currentUser).State = EntityState.Modified;
            _ctx.SetAuditAndSpecialFields();

            await _publishEndpoint.Publish(new CleanCacheForUserRequestSent()
            {
                UserId = currentUser.Id,
                TenantId = tenantId,
                AuthId = currentUser.AuthId
            }, CancellationToken.None);

            await _ctx.SaveChangesAsync();

            return currentUser;
        }

        public async Task<Either<IFeatureError, UserModel>> GetUserIfTenantLinkExistsAndValidAsync(Guid userId, Guid tenantId)
        {
            var con = _ctx.Database.GetDbConnection();

            Guid? subId = await con.QuerySingleOrDefaultAsync<Guid>
                ("""
                SELECT s.id 
                FROM subscriptions s 
                INNER JOIN tenants t ON s.id = t.subscription_id 
                WHERE t.id = @Id
                """,
                new { Id = tenantId });

            if (con.State == System.Data.ConnectionState.Open)
                await con.CloseAsync();

            if (subId == null)
                return new UserBadTenantLinkError(userId, tenantId);

            //Has link on the tenant and on the subscription
            var user = await _ctx.Users
            .FromSql($"""
                      SELECT u.* 
                      FROM users u 
                      INNER JOIN tenants_users tu ON u.Id = tu.user_id 
                      INNER JOIN subscriptions_users su ON su.user_id = u.id
                      WHERE tu.user_id = {userId} 
                      AND tu.tenant_id = {tenantId}
                      AND su.subscription_id = {subId}
                      AND su.is_activated = true
                      AND su.user_id = {userId}
                      """)
            .SingleOrDefaultAsync();

            return user == null
                ? new UserBadTenantLinkError(userId, tenantId)
                : user;
        }
    }
}
