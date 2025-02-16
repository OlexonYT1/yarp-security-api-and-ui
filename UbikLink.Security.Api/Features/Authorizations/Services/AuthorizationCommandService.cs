using LanguageExt;
using Microsoft.EntityFrameworkCore;
using UbikLink.Common.Errors;
using UbikLink.Security.Api.Data;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Mappers;

namespace UbikLink.Security.Api.Features.Authorizations.Services
{
    public class AuthorizationCommandService(SecurityDbContext ctx)
    {
        private readonly SecurityDbContext _ctx = ctx;

        public async Task<Either<IFeatureError, AuthorizationModel>> AddAuthorizationInDbAsync(AuthorizationModel authorization)
        {
            _ctx.Authorizations.Add(authorization);
            _ctx.SetAuditAndSpecialFields();
            await _ctx.SaveChangesAsync();

            return authorization;
        }

        public async Task<Either<IFeatureError, bool>> DeleteAuthorizationInDbAsync(AuthorizationModel authorization)
        {
            _ctx.Authorizations.Remove(authorization);
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<Either<IFeatureError, List<AuthorizationModel>>> GetAuthorizationsByIds(List<Guid> authorizationIds)
        {
            var authorizations = await _ctx.Authorizations.Where(a => authorizationIds.Contains(a.Id)).ToListAsync();

            return authorizations.Count != authorizationIds.Count
                ? new ResourceBatchDeleteError("AUTHORIZATION",authorizationIds.Except(authorizations.Select(a => a.Id)))
                : authorizations;
        }

        public async Task<Either<IFeatureError, AuthorizationModel>> UpdateAuthorizationInDbAsync(AuthorizationModel authorization)
        {
            _ctx.Authorizations.Update(authorization);
            _ctx.SetAuditAndSpecialFields();
            await _ctx.SaveChangesAsync();

            return authorization;
        }

        public async Task<Either<IFeatureError, bool>> DeleteAuthorizationsRangeInDbAsync(List<AuthorizationModel> authorizations)
        {
            _ctx.Authorizations.RemoveRange(authorizations);
            await _ctx.SaveChangesAsync();
            return true;
        }


        public async Task<Either<IFeatureError, AuthorizationModel>> ValidateIfExistsIdAsync(Guid currentId)
        {
            var authorization = await _ctx.Authorizations.FindAsync(currentId);

            return authorization == null
                ? new ResourceNotFoundError("Authorization", new Dictionary<string, string>()
                    {
                        {"Id", currentId.ToString()}
                    })
                : authorization;
        }

        public async Task<Either<IFeatureError, AuthorizationModel>> MapInDbContextAsync
                (AuthorizationModel current, AuthorizationModel forUpdate)
        {
            current = forUpdate.MapToAuthorization(current);
            await Task.CompletedTask;
            return current;
        }

        public async Task<Either<IFeatureError, AuthorizationModel>> ValidateIfNotAlreadyExistsAsync(AuthorizationModel current)
        {
            var authorization = await _ctx.Authorizations.SingleOrDefaultAsync(a => a.Code == current.Code);

            return authorization == null
                ? current
                : new ResourceAlreadyExistsError("Authorization", new Dictionary<string, string>()
                {
                    {"Code", current.Code}
                });
        }

        public async Task<Either<IFeatureError, AuthorizationModel>> ValidateIfNotAlreadyExistsWithOtherIdAsync(AuthorizationModel current)
        {
            var authorization = await _ctx.Authorizations.SingleOrDefaultAsync(a => a.Code == current.Code 
                && a.Id != current.Id);

            return authorization == null
                ? current
                : new ResourceAlreadyExistsError("Authorization", new Dictionary<string, string>()
                {
                    {"Code", current.Code}
                });
        }
    }
}
