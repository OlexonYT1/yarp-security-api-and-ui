using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using UbikLink.Security.Contracts.Subscriptions.Commands;
using UbikLink.Security.Contracts.Tenants.Results;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.Security.UI.Client.Components.Subscription
{
    public class UserUiObj
    {
        public Guid Id { get; set; }
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Email { get; set; }
        public bool IsActivated { get; set; } = true;
        public bool IsOwner { get; set; } = false;
        public Guid Version { get; set; }
        public bool Selected { get; set; } = false;
        public List<UserLinkedTenantSubOwnerResult> LinkedTenants { get; set; } = [];
    }

    public class TenantUserUiObj
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public string Firstname { get; set; } = default!;
        public string Lastname { get; set; } = default!;
        public bool Selected { get; set; } = false;
    }

    public static class UserMappers
    {
        public static UserUiObj ToUiObj(this UserSubOwnerResult user)
        {
            return new UserUiObj
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                IsActivated = user.IsActivated,
                IsOwner = user.IsSubscriptionOwner,
                Version = user.Version,
                Selected = false,
                LinkedTenants = [.. user.LinkedTenants.OrderBy(x => x.Label)]
            };
        }

        public static IEnumerable<UserUiObj> ToUiObjs(this IEnumerable<UserSubOwnerResult> users)
        {
            return users.Select(t => t.ToUiObj());
        }

        public static TenantUserUiObj ToTenantUserUiObj(this UserSubOwnerResult user, Guid? tenantId)
        {
            var isSelected = false;

            if (tenantId != null)
            {
                if (user.LinkedTenants != null)
                {
                    isSelected = user.LinkedTenants.Any(t => t.Id == tenantId);
                }
            }

            return new TenantUserUiObj
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Selected = isSelected
            };
        }

        public static UserUiObj ToUserUiObj(this UserWithInfoSubOwnerResult user)
        {
            return new UserUiObj
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                IsActivated = user.IsActivated,
                IsOwner = user.IsSubscriptionOwner,
                Version = user.Version,
                Selected = false,
                LinkedTenants = []
            };
        }

        public static UpdateSubscriptionLinkedUserCommand ToUpdateSubscriptionLinkedUserCommand(this UserUiObj user)
        {
            return new UpdateSubscriptionLinkedUserCommand
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                IsActivated = user.IsActivated,
                IsSubscriptionOwner = user.IsOwner,
                Version = user.Version,
            };
        }

        public static IEnumerable<TenantUserUiObj> ToTenantUserUiObjs(this IEnumerable<UserSubOwnerResult> users, Guid? tenantId)
        {
            return users.Select(t => t.ToTenantUserUiObj(tenantId));
        }
    }
}
