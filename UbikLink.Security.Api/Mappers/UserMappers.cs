using System.Runtime.CompilerServices;
using UbikLink.Security.Api.Data.Models;
using UbikLink.Security.Api.Features.Subscriptions.Services.Poco;
using UbikLink.Security.Api.Features.Tenants.Services.Poco;
using UbikLink.Security.Api.Features.Users.Services.Poco;
using UbikLink.Security.Contracts.Subscriptions.Commands;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.Security.Api.Mappers
{
    public static class UserMappers
    {
        public static UserProxyResult MapToUserProxyResult(this UserWithSubscriptionInfo user)
        {
            return new UserProxyResult
            {
                Id = user.Id,
                AuthId = user.AuthId,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                IsActivatedInSelectedSubscription = user.IsActiveInSelectedSubscription,
                IsMegaAdmin = user.IsMegaAdmin,
                SelectedTenantId = user.SelectedTenantId,
                OwnerOfSubscriptionsIds = user.OwnerOfSubscriptionsIds,
                SelectedTenantAuthorizations = [.. user.SelectedTenantAuthorizations.MapToAuthorizationLightResults()],
                SelectedTenantRoles = [.. user.SelectedTenantRoles.MapToRoleLightResults()],
                Version = user.Version
            };
        }

        public static UserMeResult MapToUserMeResult(this UserWithSubscriptionInfo user)
        {
            return new UserMeResult
            {
                Id = user.Id,
                AuthId = user.AuthId,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                IsActivatedInSelectedSubscription = user.IsActiveInSelectedSubscription,
                IsMegaAdmin = user.IsMegaAdmin,
                SelectedTenantId = user.SelectedTenantId,
                OwnerOfSubscriptionsIds = user.OwnerOfSubscriptionsIds,
                SelectedTenantAuthorizations = [.. user.SelectedTenantAuthorizations.MapToAuthorizationLightResults()],
                SelectedTenantRoles = [.. user.SelectedTenantRoles.MapToRoleLightResults()],
                Version = user.Version,
            };
        }

        public static UserStandardResult MapToUserStandardResult(this UserModel userModel)
        {
            return new UserStandardResult
            {
                Id = userModel.Id,
                Firstname = userModel.Firstname,
                Lastname = userModel.Lastname,
                Email = userModel.Email,
                Version = userModel.Version
            };
        }

        public static UserSubOwnerResult MapToUserSubOwnerResult(this UserWithLinkedTenants user)
        {
            return new UserSubOwnerResult
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                IsActivated = user.IsActivated,
                IsSubscriptionOwner = user.IsOwner,
                LinkedTenants = user.LinkedTenants.Select(x => x.MapToUserLinkedTenantSubOwnerResult()),
                Version = user.Version
            };
        }

        public static IEnumerable<UserSubOwnerResult> MapToUserSubOwnerResults(this IEnumerable<UserWithLinkedTenants> users)
        {
            return users.Select(x=>x.MapToUserSubOwnerResult());    
        }

        public static UserLinkedTenantSubOwnerResult MapToUserLinkedTenantSubOwnerResult(this UserLinkedTenant tenant)
        {
            return new UserLinkedTenantSubOwnerResult
            {
                Id = tenant.Id,
                Label = tenant.Label
            };
        }

        public static IEnumerable<UserStandardResult> MapToUserStandardResults(this IEnumerable<UserModel> userModels)
        {
            return userModels.Select(MapToUserStandardResult);
        }

        public static UserForUpd MapToUserForUpd(this UpdateSubscriptionLinkedUserCommand command)
        {
            return new UserForUpd
            {
                Firstname = command.Firstname,
                Lastname = command.Lastname,
                IsActivated = command.IsActivated,
                IsSubscriptionOwner = command.IsSubscriptionOwner,
                Version = command.Version
            };
        }

        public static (UserModel User, SubscriptionUserModel SubUser) MapToUserAndSubUser(this UserForUpd forUpd, 
            UserModel user, 
            SubscriptionUserModel subUser)
        {
            user.Firstname = forUpd.Firstname;
            user.Lastname = forUpd.Lastname;
            user.Version = forUpd.Version;

            subUser.IsOwner = forUpd.IsSubscriptionOwner;
            subUser.IsActivated = forUpd.IsActivated;

            return (user, subUser);
        }

        public static UserWithInfoSubOwnerResult MapToUserWithInfoSubOwnerResult(this (UserModel User, SubscriptionUserModel SubUser) userAndSubUser)
        {
            return new UserWithInfoSubOwnerResult
            {
                Id = userAndSubUser.User.Id,
                Firstname = userAndSubUser.User.Firstname,
                Lastname = userAndSubUser.User.Lastname,
                Email = userAndSubUser.User.Email,
                IsActivated = userAndSubUser.SubUser.IsActivated,
                IsSubscriptionOwner = userAndSubUser.SubUser.IsOwner,
                Version = userAndSubUser.User.Version
            };
        }

        public static UserWithTenantRoleIdsResult MapToUserWithTenantRoleIdsResult(this UserWithLinkedRoles user)
        {
            return new UserWithTenantRoleIdsResult
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                TenantRoleIds = user.LinkedRoleIds,
                Version = user.Version,
            };
        }

        public static IEnumerable<UserWithTenantRoleIdsResult> MapToUserWithTenantRoleIdsResult(this IEnumerable<UserWithLinkedRoles> user)
        {
            return user.Select(MapToUserWithTenantRoleIdsResult);
        }
    }
}
