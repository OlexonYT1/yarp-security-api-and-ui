using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using UbikLink.Proxy.Services;

namespace UbikLink.Proxy.Authorizations
{
    public enum PermissionMode
    {
        Role,
        Authorization
    }

    public class UserTenantRolesOrAuthorizationsRequirement(string[] values, PermissionMode mode) : IAuthorizationRequirement
    {
        public string[] Values { get; set; } = values;
        public PermissionMode Mode { get; set; } = mode;
    }

    public class UserRolesAuthorizationOkHandler(UserService userService)
        : AuthorizationHandler<UserTenantRolesOrAuthorizationsRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            UserTenantRolesOrAuthorizationsRequirement requirement)
        {
            var authId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authId == null)
            {
                context.Fail();
                return;
            }

            var userInfo = await userService.GetUserInfoAsync(authId);

            if (userInfo == null)
            {
                context.Fail();
                return;
            }

            if (!userInfo.IsActivatedInSelectedSubscription)
            {
                context.Fail();
                return;
            }

            //TODO: it's a security hole (need to check)
            // Certain tenant authorizations don't need to be deleted or you will be locked out of a tenant...
            //if (requirement.IsSubscriptionOwnerAllowed
            //               && userInfo.OwnerOfSubscriptionsIds.Any())
            //{
            //    context.Succeed(requirement);
            //    return;
            //}

            switch (requirement.Mode)
            {
                case PermissionMode.Role:
                    if (!requirement.Values.Except(userInfo!.SelectedTenantRoles.Select(r => r.Code)).Any())
                    {
                        context.Succeed(requirement);
                        return;
                    }
                    break;
                case PermissionMode.Authorization:
                    if (!requirement.Values.Except(userInfo!.SelectedTenantAuthorizations.Select(a => a.Code)).Any())
                    {
                        context.Succeed(requirement);
                        return;
                    }
                    break;
            }

            context.Fail();
            return;
        }
    }
}
