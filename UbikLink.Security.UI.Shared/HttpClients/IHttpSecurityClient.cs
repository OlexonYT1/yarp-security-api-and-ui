using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Roles.Commands;
using UbikLink.Security.Contracts.Subscriptions.Commands;
using UbikLink.Security.Contracts.Users.Commands;

namespace UbikLink.Security.UI.Shared.Httpclients
{
    public interface IHttpSecurityClient
    {
        Task<HttpResponseMessage> GetAllAuthorizationsForAdminAsync();
        Task<HttpResponseMessage> GetAuthorizationForAdminByIdAsync(Guid id);
        Task<HttpResponseMessage> AddAuthorizationForAdminAsync(AddAuthorizationCommand command);
        Task<HttpResponseMessage> UpdateAuthorizationForAdminAsync(Guid id, UpdateAuthorizationCommand command);
        Task<HttpResponseMessage> DeleteAuthorizationForAdminAsync(Guid id);
        Task<HttpResponseMessage> BatchDeleteAuthorizationsForAdminAsync(BatchDeleteAuthorizationCommand command);

        Task<HttpResponseMessage> GetAllRolesForAdminAsync();
        Task<HttpResponseMessage> GetRoleForAdminByIdAsync(Guid id);
        Task<HttpResponseMessage> AddRoleForAdminAsync(AddRoleCommand command);
        Task<HttpResponseMessage> UpdateRoleForAdminAsync(Guid id, UpdateRoleCommand command);
        Task<HttpResponseMessage> DeleteRoleForAdminAsync(Guid id);
        Task<HttpResponseMessage> BatchDeleteRolesForAdminAsync(BatchDeleteRoleCommand command);

        Task<HttpResponseMessage> GetAllSubscribtionsForUserAsync();
        Task<HttpResponseMessage> GetSelectedSubscriptionForOwnerAsync();
        Task<HttpResponseMessage> GetSelectedSubscriptionLinkedTenantsForSubOwnerAsync();
        Task<HttpResponseMessage> GetSelectedSubscriptionLinkedUsersForSubOwnerAsync();
        Task<HttpResponseMessage> AddTenantInSelectedSubscriptionForSubOwnerAsync(AddSubscriptionLinkedTenantCommand command);
        Task<HttpResponseMessage> UpdateTenantInSelectedSubscriptionForSubOwnerAsync(Guid id, UpdateSubscriptionLinkedTenantCommand command);
        Task<HttpResponseMessage> GetSubscriptionLinkedTenantForSubOwnerAsync(Guid tenantId);
        Task<HttpResponseMessage> GetSubscriptionLinkedUserForSubOwnerAsync(Guid userId);
        Task<HttpResponseMessage> DeleteSubscriptionLinkedTenantForSubOwnerAsync(Guid id);
        Task<HttpResponseMessage> UpdateUserInSelectedSubscriptionForSubOwnerAsync(Guid id, UpdateSubscriptionLinkedUserCommand command);

        Task<HttpResponseMessage> GetAllTenantsForUserAsync();
        
        Task<HttpResponseMessage> GetTenantAsync(Guid id);
        Task<HttpResponseMessage> GetTenantUsersWithRolesIdsAsync(Guid id);
        Task<HttpResponseMessage> GetTenantUserWithRolesIdsAsync(Guid id, Guid userId);
        Task<HttpResponseMessage> GetTenantAvailableRolesAsync(Guid id);
        Task<HttpResponseMessage> UpdateTenantUserRolesAsync(Guid id, Guid userId, UpdateUserWithTenantRoleIdsCommand command);

        Task<HttpResponseMessage> GetConnectedUserInfo();
        Task<HttpResponseMessage> SetUserSettings(SetSettingsUserMeCommand command);
    }
}
