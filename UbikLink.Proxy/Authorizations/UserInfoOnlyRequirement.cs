using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using UbikLink.Proxy.Services;

namespace UbikLink.Proxy.Authorizations
{
    public enum RoleRequirement
    {
        MegaAdmin,
        SubscriptionOwner,
        User
    }

    public class UserInfoOnlyRequirement(RoleRequirement roleType) : IAuthorizationRequirement
    {
        public RoleRequirement RoleType { get; set; } = roleType;
    }
    public class UserInfoOkHandler(UserService userService) : AuthorizationHandler<UserInfoOnlyRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserInfoOnlyRequirement requirement)
        {
            var authId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authId != null)
            {
                var userInfo = await userService.GetUserInfoAsync(authId);

                switch (requirement.RoleType)
                {
                    case RoleRequirement.MegaAdmin:
                        if (userInfo != null 
                            && userInfo.IsMegaAdmin)
                        {
                            context.Succeed(requirement);
                            return;
                        }
                        break;

                    case RoleRequirement.User:
                        if (userInfo != null)
                        {
                            context.Succeed(requirement);
                            return;
                        }
                        break;

                    case RoleRequirement.SubscriptionOwner:
                        if (userInfo != null 
                            && userInfo.OwnerOfSubscriptionsIds.Any())
                        {
                            context.Succeed(requirement);
                            return;
                        }
                        break;
                }
            }
            context.Fail();
            return;
        }
    }
}
