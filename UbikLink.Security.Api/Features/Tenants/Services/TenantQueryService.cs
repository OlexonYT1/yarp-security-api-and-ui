using Dapper;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Roles.Services.Poco;
using UbikLink.Security.Api.Features.Tenants.Services.Poco;

namespace UbikLink.Security.Api.Features.Tenants.Services
{
    public class TenantQueryService(SecurityDbContext ctx)
    {
        private readonly SecurityDbContext _ctx = ctx;

        public async Task<IEnumerable<TenantModel>> GetAllTenantsAsync()
        {
            return await _ctx.Tenants.ToListAsync();
        }

        public async Task<Either<IFeatureError, TenantModel>> GetTenantByIdAsync(Guid id)
        {
            var result = await _ctx.Tenants.FindAsync(id);

            return result == null || result.IsActivated == false
                ? new ResourceNotFoundError("Tenant", new Dictionary<string, string>()
                {
                    {"Id", id.ToString()}
                })
                : result;
        }

        public async Task<Either<IFeatureError, TenantModel>> GetTenantByIdForAdminAsync(Guid id)
        {
            var result = await _ctx.Tenants.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("Tenant", new Dictionary<string, string>()
                {
                    {"Id", id.ToString()}
                })
                : result;
        }

        public async Task<IEnumerable<TenantModel>> GetAllTenantsForUserAsync(Guid userId)
        {
            var tenants = await _ctx.Tenants
                .FromSql($"""
                         SELECT t.* 
                         FROM tenants t 
                         INNER JOIN tenants_users tu ON t.Id = tu.tenant_id 
                         INNER JOIN subscriptions_users su ON tu.user_id = su.user_id 
                         AND t.subscription_id = su.subscription_id
                         WHERE tu.user_id = {userId}
                         AND su.is_activated = true
                         """)
                .ToListAsync();
            return tenants;
        }

        public async Task<Either<IFeatureError, IEnumerable<UserWithLinkedRoles>>> GetAllTenantUsersWithRolesAsync(TenantModel tenant)
        {
            var con = _ctx.Database.GetDbConnection();
            var sql = $"""
                       SELECT u.*
                       FROM users u
                       INNER JOIN tenants_users tu ON u.id = tu.user_id
                       WHERE tu.tenant_id = @tenantId;
                       SELECT tu.user_id, tur.role_id
                       FROM tenants_users_roles tur
                       INNER JOIN tenants_users tu ON tu.id = tur.tenant_user_id
                       INNER JOIN roles r ON r.id = tur.role_id
                       WHERE tu.tenant_id = @tenantId; 
                       """;

            var result = new List<UserWithLinkedRoles>();

            using (var multi = await con.QueryMultipleAsync(sql, new { tenantId = tenant.Id }))
            {
                result = [.. await multi.ReadAsync<UserWithLinkedRoles>()];

                var roleLink = (await multi.ReadAsync<(Guid UserId, Guid RoleId)>())
                    .GroupBy(x => x.UserId)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.RoleId).ToList());

                foreach (var user in result)
                {
                    if (roleLink.TryGetValue(user.Id, out var ids))
                    {
                        user.LinkedRoleIds.AddRange(ids);
                    }
                }
            }

            if (con.State == System.Data.ConnectionState.Open)
                await con.CloseAsync();

            return result;
        }

        public async Task<Either<IFeatureError,List<RoleModel>>>
            GetTenantAvailableRoles(TenantModel tenant)
        {
            var roles = await _ctx.Roles.Where(x=>x.TenantId == tenant.Id || x.TenantId == null).ToListAsync();

            return roles;
        }

        public async Task<Either<IFeatureError, UserWithLinkedRoles>> GetTenantUserWithRolesAsync(TenantModel tenant
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

                if(result == null)
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

            return result;
        }
    }
}
