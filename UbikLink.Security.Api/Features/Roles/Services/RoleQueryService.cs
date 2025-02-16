using LanguageExt;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Data;
using UbikLink.Security.Api.Features.Roles.Services.Poco;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace UbikLink.Security.Api.Features.Roles.Services
{
    public class RoleQueryService(SecurityDbContext ctx)
    {
        private readonly SecurityDbContext _ctx = ctx;

        public async Task<List<RoleWithAuthorizationIds>> GetAllRolesForAdminAsync()
        {
            var con = _ctx.Database.GetDbConnection();
            var sql = $"""
                       SELECT r.*
                       FROM roles r
                       WHERE tenant_id IS NULL;
                       SELECT r.id, ra.authorization_id
                       FROM roles_authorizations ra
                       INNER JOIN roles r ON r.id = ra.role_id
                       WHERE r.tenant_id IS NULL;
                       """;

            var result = new List<RoleWithAuthorizationIds>();

            using (var multi = await con.QueryMultipleAsync(sql))
            {
                result = [.. (await multi.ReadAsync<RoleWithAuthorizationIds>())];

                var authIds = (await multi.ReadAsync<(Guid RoleId, Guid AuthId)>())
                    .GroupBy(x => x.RoleId)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.AuthId).ToList());

                foreach (var role in result)
                {
                    if (authIds.TryGetValue(role.Id, out var ids))
                    {
                        role.AuthorizationIds.AddRange(ids);
                    }
                }
            }

            if (con.State == System.Data.ConnectionState.Open)
                await con.CloseAsync();

            return result;
        }

        public async Task<Either<IFeatureError, RoleWithAuthorizationIds>> GetRoleForAdminAsync(Guid id)
        {
            var con = _ctx.Database.GetDbConnection();
            var sql = $"""
                       SELECT r.*
                       FROM roles r
                       WHERE r.id = @id
                       AND tenant_id IS NULL;
                       SELECT ra.authorization_id
                       FROM roles_authorizations ra
                       WHERE ra.role_id = @id;
                       """;

            var result = new RoleWithAuthorizationIds()
            {
                Code = string.Empty
            };

            using (var multi = await con.QueryMultipleAsync(sql, new { id }))
            {
                result = await multi.ReadFirstOrDefaultAsync<RoleWithAuthorizationIds>();

                if (result == null)
                    return new ResourceNotFoundError("Role", new Dictionary<string, string>()
                    {
                        { "Id", id.ToString() }
                     });

                var authIds = await multi.ReadAsync<Guid>();
                result.AuthorizationIds.AddRange(authIds);
            }

            if (con.State == System.Data.ConnectionState.Open)
                await con.CloseAsync();

            return result;
        }
    }
}
