using System.ComponentModel.DataAnnotations;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Authorizations.Results;

namespace UbikLink.Security.UI.Client.Components.Shared
{
    public class AuthorizationUiObj
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Code too long (50 character limit).")]
        public string Code { get; set; } = default!;

        [StringLength(500, ErrorMessage = "Description too long (500 character limit).")]
        public string? Description { get; set; }

        public bool Selected { get; set; } = false;
        public Guid Version { get; set; }
    }

    public static class AuthorizationUiObjMappers
    {
        public static AuthorizationUiObj MapToAuthorizationUiObj(this AuthorizationStandardResult result)
        {
            return new AuthorizationUiObj
            {
                Id = result.Id,
                Code = result.Code,
                Description = result.Description,
                Version = result.Version
            };
        }

        public static IEnumerable<AuthorizationUiObj> MapToAuthorizationUiObjs(this IEnumerable<AuthorizationStandardResult> results)
        {
            return results.Select(result => result.MapToAuthorizationUiObj());
        }

        public static AuthorizationStandardResult MapToAuthorizationStandardResult(this AuthorizationUiObj uiObj)
        {
            return new AuthorizationStandardResult
            {
                Id = uiObj.Id,
                Code = uiObj.Code,
                Description = uiObj.Description,
                Version = uiObj.Version
            };
        }

        public static IEnumerable<AuthorizationStandardResult> MapToAuthorizationStandardResults(this IEnumerable<AuthorizationUiObj> uiObjs)
        {
            return uiObjs.Select(uiObj => uiObj.MapToAuthorizationStandardResult());
        }

        public static AddAuthorizationCommand MapToAddAuthorizationCommand(this AuthorizationUiObj uiObj)
        {
            return new AddAuthorizationCommand
            {
                Code = uiObj.Code,
                Description = uiObj.Description
            };
        }

        public static UpdateAuthorizationCommand MapToUpdateAuthorizationCommand(this AuthorizationUiObj uiObj)
        {
            return new UpdateAuthorizationCommand
            {
                Version = uiObj.Version,
                Code = uiObj.Code,
                Description = uiObj.Description
            };
        }
    }
}
