using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Authorizations.Results;

namespace UbikLink.Security.Api.Mappers
{
    public static class AuthorizationMappers
    {
        public static AuthorizationModel MapToAuthorization(this AddAuthorizationCommand entity)
        {
            return new AuthorizationModel
            {
                Code = entity.Code,
                Description = entity.Description
            };
        }

        public static AuthorizationModel MapToAuthorization(this UpdateAuthorizationCommand entity, Guid currentId)
        {
            return new AuthorizationModel
            {
                Id = currentId,
                Code = entity.Code,
                Description = entity.Description,
                Version = entity.Version,
            };
        }

        public static AuthorizationLightResult MapToAuthorizationLightResult(this AuthorizationModel current)
        {
            return new AuthorizationLightResult
            {
                Id = current.Id,
                Code = current.Code
            };
        }

        public static IEnumerable<AuthorizationLightResult> MapToAuthorizationLightResults(this IEnumerable<AuthorizationModel> current)
        {
            return current.Select(MapToAuthorizationLightResult);
        }

        public static AuthorizationModel MapToAuthorization(this AuthorizationModel forUpd, AuthorizationModel model)
        {
            model.Id = forUpd.Id;
            model.Code = forUpd.Code;
            model.Description = forUpd.Description;
            model.Version = forUpd.Version;

            return model;
        }

        public static AuthorizationStandardResult MapToAuthorizationStandardResult(this AuthorizationModel model)
        {
            return new AuthorizationStandardResult
            {
                Id = model.Id,
                Code = model.Code,
                Description = model.Description,
                Version = model.Version
            };
        }

        public static IEnumerable<AuthorizationStandardResult> MapToAuthorizationStandardResults(this IEnumerable<AuthorizationModel> models)
        {
            return models.Select(model => new AuthorizationStandardResult
            {
                Id = model.Id,
                Code = model.Code,
                Description = model.Description,
                Version = model.Version
            });
        }
    }
}
