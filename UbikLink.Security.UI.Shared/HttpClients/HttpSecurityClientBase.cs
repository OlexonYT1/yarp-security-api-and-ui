using System.Net.Http.Json;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Roles.Commands;
using UbikLink.Security.Contracts.Subscriptions.Commands;
using UbikLink.Security.Contracts.Users.Commands;

namespace UbikLink.Security.UI.Shared.Httpclients
{
    public class HttpSecurityClientBase(HttpClient client) : IHttpSecurityClient
    {
        protected readonly HttpClient Client = client;

        public async Task<HttpResponseMessage> GetAllAuthorizationsForAdminAsync()
        {
            return await SendAsync(() => Client.GetAsync("admin/authorizations"));
        }

        public async Task<HttpResponseMessage> AddAuthorizationForAdminAsync(AddAuthorizationCommand command)
        {
            return await SendAsync(() => Client.PostAsJsonAsync("admin/authorizations", command));
        }

        public async Task<HttpResponseMessage> UpdateAuthorizationForAdminAsync(Guid id, UpdateAuthorizationCommand command)
        {
            return await SendAsync(() => Client.PutAsJsonAsync($"admin/authorizations/{id}", command));
        }

        public async Task<HttpResponseMessage> DeleteAuthorizationForAdminAsync(Guid id)
        {
            return await SendAsync(() => Client.DeleteAsync($"admin/authorizations/{id}"));
        }

        public async Task<HttpResponseMessage> BatchDeleteAuthorizationsForAdminAsync(BatchDeleteAuthorizationCommand command)
        {
            return await SendAsync(() => Client.PostAsJsonAsync($"admin/authorizations/commands/batch-delete", command));
        }

        public async Task<HttpResponseMessage> GetAuthorizationForAdminByIdAsync(Guid id)
        {
            return await SendAsync(() => Client.GetAsync($"admin/authorizations/{id}"));
        }

        public async Task<HttpResponseMessage> GetAllSubscribtionsForUserAsync()
        {
            return await SendAsync(() => Client.GetAsync($"me/subscriptions"));
        }

        public async Task<HttpResponseMessage> GetAllTenantsForUserAsync()
        {
            return await SendAsync(() => Client.GetAsync($"me/tenants"));
        }

        public async Task<HttpResponseMessage> GetConnectedUserInfo()
        {
            return await SendAsync(() => Client.GetAsync($"me/authinfo"));
        }

        public async Task<HttpResponseMessage> SetUserSettings(SetSettingsUserMeCommand command)
        {
            return await SendAsync(() => Client.PostAsJsonAsync($"me/selecttenant", command));
        }
        public async Task<HttpResponseMessage> GetSelectedSubscriptionForOwnerAsync()
        {
            return await SendAsync(() => Client.GetAsync($"subowner/subscriptions/selected"));
        }
        public async Task<HttpResponseMessage> GetSelectedSubscriptionLinkedTenantsForSubOwnerAsync()
        {
            return await SendAsync(() => Client.GetAsync($"subowner/subscriptions/selected/tenants"));
        }

        public async Task<HttpResponseMessage> GetSelectedSubscriptionLinkedUsersForSubOwnerAsync()
        {
            return await SendAsync(() => Client.GetAsync($"subowner/subscriptions/selected/users"));
        }

        public async Task<HttpResponseMessage> AddTenantInSelectedSubscriptionForSubOwnerAsync(AddSubscriptionLinkedTenantCommand command)
        {
            return await SendAsync(() => Client.PostAsJsonAsync($"subowner/subscriptions/selected/tenants", command));
        }

        public async Task<HttpResponseMessage> GetSubscriptionLinkedTenantForSubOwnerAsync(Guid tenantId)
        {
            return await SendAsync(() => Client.GetAsync($"subowner/subscriptions/selected/tenants/{tenantId}"));
        }

        public async Task<HttpResponseMessage> UpdateTenantInSelectedSubscriptionForSubOwnerAsync(Guid id, UpdateSubscriptionLinkedTenantCommand command)
        {
            return await SendAsync(() => Client.PutAsJsonAsync($"subowner/subscriptions/selected/tenants/{id}", command));
        }

        public async Task<HttpResponseMessage> DeleteSubscriptionLinkedTenantForSubOwnerAsync(Guid id)
        {
            return await SendAsync(() => Client.DeleteAsync($"subowner/subscriptions/selected/tenants/{id}"));
        }

        public async Task<HttpResponseMessage> UpdateUserInSelectedSubscriptionForSubOwnerAsync(Guid id, UpdateSubscriptionLinkedUserCommand command)
        {
            return await SendAsync(() => Client.PutAsJsonAsync($"subowner/subscriptions/selected/users/{id}", command));
        }

        public async Task<HttpResponseMessage> GetSubscriptionLinkedUserForSubOwnerAsync(Guid userId)
        {
            return await SendAsync(() => Client.GetAsync($"subowner/subscriptions/selected/users/{userId}"));
        }

        public async Task<HttpResponseMessage> GetAllRolesForAdminAsync()
        {
            return await SendAsync(() => Client.GetAsync("admin/roles"));
        }

        public async Task<HttpResponseMessage> GetRoleForAdminByIdAsync(Guid id)
        {
            return await SendAsync(() => Client.GetAsync($"admin/roles/{id}"));
        }

        public async Task<HttpResponseMessage> AddRoleForAdminAsync(AddRoleCommand command)
        {
            return await SendAsync(() => Client.PostAsJsonAsync("admin/roles", command));
        }

        public async Task<HttpResponseMessage> UpdateRoleForAdminAsync(Guid id, UpdateRoleCommand command)
        {
            return await SendAsync(() => Client.PutAsJsonAsync($"admin/roles/{id}", command));
        }

        public async Task<HttpResponseMessage> DeleteRoleForAdminAsync(Guid id)
        {
            return await SendAsync(() => Client.DeleteAsync($"admin/roles/{id}"));
        }

        public async Task<HttpResponseMessage> BatchDeleteRolesForAdminAsync(BatchDeleteRoleCommand command)
        {
            return await SendAsync(() => Client.PostAsJsonAsync($"admin/roles/commands/batch-delete", command));
        }

        public async Task<HttpResponseMessage> GetTenantAsync(Guid id)
        {
            return await SendAsync(() => Client.GetAsync($"tenants/{id}"));
        }

        public async Task<HttpResponseMessage> GetTenantUsersWithRolesIdsAsync(Guid id)
        {
            return await SendAsync(() => Client.GetAsync($"tenants/{id}/users"));
        }

        public async Task<HttpResponseMessage> GetTenantAvailableRolesAsync(Guid id)
        {
            return await SendAsync(() => Client.GetAsync($"tenants/{id}/roles"));
        }

        public async Task<HttpResponseMessage> GetTenantUserWithRolesIdsAsync(Guid id, Guid userId)
        {
            return await SendAsync(() => Client.GetAsync($"tenants/{id}/users/{userId}"));
        }

        public async Task<HttpResponseMessage> UpdateTenantUserRolesAsync(Guid id, Guid userId, UpdateUserWithTenantRoleIdsCommand command)
        {
            return await SendAsync(() => Client.PostAsJsonAsync($"tenants/{id}/users/{userId}/update-roles", command));
        }

        protected virtual async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> httpRequest)
        {
            return await httpRequest();
        }
    }
}
