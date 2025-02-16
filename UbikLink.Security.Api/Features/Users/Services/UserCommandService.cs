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
                new {Id = tenantId});

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
