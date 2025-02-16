using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Roles.Errors;
using UbikLink.Security.Api.Features.Roles.Services.Poco;
using UbikLink.Security.Api.Mappers;
using UbikLink.Security.Contracts.Roles.Events;

namespace UbikLink.Security.Api.Features.Roles.Services
{
    public class RoleCommandService(SecurityDbContext ctx, IPublishEndpoint publishEndpoint)
    {
        private readonly SecurityDbContext _ctx = ctx;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

        public async Task<Either<IFeatureError,RoleWithAuthorizationIds>> 
            AddRoleForAdminInDbAsync(RoleModel role, List<Guid> linkedAuthorizationIds)
        {
            await _ctx.Roles.AddAsync(role);
            await _ctx.RolesAuthorizations.AddRangeAsync(linkedAuthorizationIds.Select(x=>

            new RoleAuthorizationModel()
            {
                Id = NewId.NextGuid(),
                RoleId = role.Id,
                AuthorizationId = x
            }));

            _ctx.SetAuditAndSpecialFields();
            await _ctx.SaveChangesAsync();

            return new RoleWithAuthorizationIds()
            {
                Id = role.Id,
                Code = role.Code,
                Description = role.Description,
                Version = role.Version,
                AuthorizationIds = linkedAuthorizationIds
            };
        }

        public async Task<Either<IFeatureError, RoleWithAuthorizationIds>>
            UpdateRoleForAdminAsync(RoleModel role, List<Guid> authIdsToAttach, List<Guid> authIdsToDetach)
        {
            var strategy = _ctx.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using (var transaction = await _ctx.Database.BeginTransactionAsync())
                {
                    _ctx.Entry(role).State = EntityState.Modified;

                    await _ctx.RolesAuthorizations
                        .Where(x => authIdsToDetach.Contains(x.AuthorizationId) && x.RoleId == role.Id)
                        .ExecuteDeleteAsync();

                    await _ctx.RolesAuthorizations.AddRangeAsync(authIdsToAttach.Select(authId => new RoleAuthorizationModel()
                    {
                        Id = NewId.NextGuid(),
                        RoleId = role.Id,
                        AuthorizationId = authId
                    }));

                    _ctx.SetAuditAndSpecialFields();

                    //publish info for cache cleaning
                    await _publishEndpoint.Publish(new CleanCacheRoleUpdated()
                    {
                        RoleId = role.Id,
                        TenantId = null
                    });

                    await _ctx.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                var autIds = await _ctx.RolesAuthorizations
                    .Where(x => x.RoleId == role.Id)
                    .Select(x => x.AuthorizationId)
                    .ToListAsync();

                return new RoleWithAuthorizationIds()
                {
                    Id = role.Id,
                    Code = role.Code,
                    Description = role.Description,
                    Version = role.Version,
                    AuthorizationIds = autIds,
                };
            });
        }
        public async Task<Either<IFeatureError, bool>> DeleteRoleInDbAsync(RoleModel role)
        {
            _ctx.Roles.Remove(role);
            await _publishEndpoint.Publish(new CleanCacheRoleDeleted()
            {
                RoleId = role.Id,
                TenantId = null
            });
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<Either<IFeatureError, bool>> DeleteRolesRangeInDbAsync(List<RoleModel> roles)
        {
            _ctx.Roles.RemoveRange(roles);
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<Either<IFeatureError, List<RoleModel>>> GetRolesByIdsForAdmin(List<Guid> rolesIds)
        {
            var roles = await _ctx.Roles.Where(a => rolesIds.Contains(a.Id) && a.TenantId == null).ToListAsync();

            return roles.Count != rolesIds.Count
                ? new ResourceBatchDeleteError("ROLE", rolesIds.Except(roles.Select(a => a.Id)))
                : roles;
        }

        public async Task<Either<IFeatureError, (RoleModel Role,List<Guid>AuthorizationIds)>> 
            ValidateAuthorizationIdsList(RoleModel role, List<Guid> authorizationIds)
        {
            var authorizations = await _ctx.Authorizations.Where(a => authorizationIds.Contains(a.Id)).ToListAsync();

            return authorizations.Count != authorizationIds.Count
                ? new BadAuthorizationIdsError(authorizationIds.Except(authorizations.Select(a => a.Id)))
                : (role, authorizationIds);
        }

        public async Task<Either<IFeatureError, (RoleModel Role, List<Guid> AttachIds, List<Guid> DetachIds)>>
            PrepareAuthorizationIdsForAttachAndDetachForUpdAsync(RoleModel role, List<Guid> authorizationIds)
        {
            var idsToAttach = new List<Guid>();
            var idsToDetach = new List<Guid>();

            //Get all auth in role
            var roleAuthIds = await _ctx.RolesAuthorizations
                .Where(su => su.RoleId == role.Id)
                .Select(su => su.AuthorizationId)
                .ToListAsync();

            //Prepare auth for attach
            foreach (var authId in authorizationIds)
            {
                if (!roleAuthIds.Contains(authId))
                    idsToAttach.Add(authId);
            }

            //Prepare users for detach (when update)
            idsToDetach.AddRange(roleAuthIds.Except(authorizationIds));

            return (role, idsToAttach, idsToDetach);
        }

        public async Task<Either<IFeatureError, RoleModel>> MapInDbContextAsync
        (RoleModel current, RoleModel forUpdate)
        {
            current = forUpdate.MapToRole(current);
            await Task.CompletedTask;
            return current;
        }

        public async Task<Either<IFeatureError, RoleModel>> ValidateIfExistsIdAsync(Guid currentId)
        {
            var role = await _ctx.Roles.FindAsync(currentId);

            return role == null
                ? new ResourceNotFoundError("Role", new Dictionary<string, string>()
                    {
                        {"Id", currentId.ToString()}
                    })
                : role;
        }

        public async Task<Either<IFeatureError, RoleModel>> ValidateIfNotAlreadyExistsForAdminAsync(RoleModel current)
        {
            var result = await _ctx.Roles.SingleOrDefaultAsync(a => a.Code == current.Code && a.TenantId == null);

            return result == null
                ? current
                : new ResourceAlreadyExistsError("Role", new Dictionary<string, string>()
                {
                    {"Code", current.Code}
                });
        }

        public async Task<Either<IFeatureError, RoleModel>> ValidateIfNotAlreadyExistsWithOtherIdForAdminAsync(RoleModel current)
        {
            var role = await _ctx.Roles.SingleOrDefaultAsync(a => a.Code == current.Code
                && a.Id != current.Id && a.TenantId == null);

            return role == null
                ? current
                : new ResourceAlreadyExistsError("Role", new Dictionary<string, string>()
                {
                    {"Code", current.Code}
                });
        }
    }
}
