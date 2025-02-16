using Dapper;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Subscriptions.Services.Poco;
using UbikLink.Security.Api.Features.Users.Services.Poco;

namespace UbikLink.Security.Api.Features.Users.Services
{
    public class UserQueryService(SecurityDbContext ctx)
    {
        private readonly SecurityDbContext _ctx = ctx;

        public async Task<Either<IFeatureError, UserModel>> GetUserByAuthId(string authId)
        {
            var result = await _ctx.Users.SingleOrDefaultAsync(u => u.AuthId == authId);

            return result == null
                ? new ResourceNotFoundError("User", new Dictionary<string, string>()
                {
                    {"OAuthId", authId}
                })
                : result;
        }

        public async Task<Either<IFeatureError, UserWithSubscriptionInfo>>
            IsActivatedInSelectedSubscription(UserModel user)
        {
            var con = _ctx.Database.GetDbConnection();
            var sql = $"""
                       SELECT su.is_activated 
                       FROM subscriptions_users su
                       INNER JOIN users u ON su.user_id = u.id
                       INNER JOIN tenants t ON t.id = u.selected_tenant_id
                       WHERE u.id = @userId
                       AND t.id = @selectedTenantId
                       AND su.subscription_id = t.subscription_id
                       """;

            var active = await con.QuerySingleOrDefaultAsync<bool?>(sql, new { userId = user.Id, user.SelectedTenantId }) ?? false;

            if (con.State == System.Data.ConnectionState.Open)
                await con.CloseAsync();

            return new UserWithSubscriptionInfo()
            {
                AuthId = user.AuthId,
                Id = user.Id,
                IsActiveInSelectedSubscription = active,
                SelectedTenantId = user.SelectedTenantId,
                Email = user.Email,
                Firstname = user.Firstname,
                IsMegaAdmin = user.IsMegaAdmin,
                Lastname = user.Lastname,
                Version = user.Version
            };
        }

        public async Task<Either<IFeatureError, UserModel>> GetUserById(Guid id)
        {
            var result = await _ctx.Users.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("User", new Dictionary<string, string>()
                {
                    {"Id", id.ToString()}
                })
                : result;
        }

        public async Task<Either<IFeatureError, UserWithSubscriptionInfo>> FillOwnedSubscribtionIdsAsync(UserWithSubscriptionInfo user)
        {
            var result = await _ctx.SubscribtionsUsers
                .Where(o => o.UserId == user.Id && o.IsOwner && o.IsActivated)
                .Select(o => o.SubscriptionId)
                .ToListAsync();

            user.OwnerOfSubscriptionsIds = result;

            return user;
        }

        public async Task<Either<IFeatureError, UserWithSubscriptionInfo>> GetTenantRolesAndAuthorizationsAsync(UserWithSubscriptionInfo user)
        {
            var con = _ctx.Database.GetDbConnection();
            var sql = $"""
                       SELECT r.id as role_id, 
                       r.code as role_code, 
                       a.id as authorization_id,
                       a.code as authorization_code
                       FROM tenants_users_roles tur
                       INNER JOIN tenants_users tu ON tu.id = tur.tenant_user_id
                       INNER JOIN tenants t ON t.id = tu.tenant_id
                       INNER JOIN roles r ON r.id = tur.role_id
                       INNER JOIN roles_authorizations ra ON ra.role_id = r.id
                       INNER JOIN authorizations a ON ra.authorization_id = a.id
                       WHERE tu.tenant_id = @tenantId
                       AND tu.user_id = @userId
                       AND t.is_activated = true;
                       """;

            var rolesAndAuths = await con.QueryAsync<dynamic>(sql, new { userId = user.Id, tenantId = user.SelectedTenantId });

            if (con.State == System.Data.ConnectionState.Open)
                await con.CloseAsync();

            var dictRole = new Dictionary<Guid, RoleModel>();
            var dictAuth = new Dictionary<Guid, AuthorizationModel>();

            foreach (var entry in rolesAndAuths)
            {
                if(!dictRole.ContainsKey(entry.role_id))
                    dictRole[entry.role_id] = new RoleModel()
                    {
                        Id = entry.role_id,
                        Code = entry.role_code
                    };
                
                if(!dictAuth.ContainsKey(entry.authorization_id))
                    dictAuth[entry.authorization_id] = new AuthorizationModel()
                    {
                        Id = entry.authorization_id,
                        Code = entry.authorization_code
                    };
            }

            user.SelectedTenantRoles = [.. dictRole.Values];
            user.SelectedTenantAuthorizations = [.. dictAuth.Values];

            return user;
        }

        public async Task<Either<IFeatureError, (UserModel User, List<Guid> OwnedSubscriptionIds)>>
            GetOwnedSubscribtionIds(UserModel user)
        {
            var result = await _ctx.SubscribtionsUsers
                .Where(o => o.UserId == user.Id && o.IsOwner && o.IsActivated)
                .Select(o => o.SubscriptionId)
                .ToListAsync();

            return (user, result);
        }
    }
}
