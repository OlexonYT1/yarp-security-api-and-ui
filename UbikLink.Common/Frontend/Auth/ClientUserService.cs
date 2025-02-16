using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Common.Frontend.Auth;
using UbikLink.Common.Frontend.HttpClients;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.Common.Frontend.Auth
{
    public class ClientUserService(UserAndTokenCache userAndTokenCache, IHttpUserClient client)
    {
        private readonly UserAndTokenCache _userAndTokenCache = userAndTokenCache;
        private readonly IHttpUserClient _client = client;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public ClaimsPrincipal GetUser()
        {
            return _currentUser;
        }

        public async Task<string> GetCurrentUserTokenAsync(string? userAuthId = null)
        {
            userAuthId ??= (_currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (string.IsNullOrEmpty(userAuthId))
            {
                return string.Empty;
            }

            var token = await _userAndTokenCache.GetUserTokenAsync(userAuthId);

            return token == null
                ? string.Empty
                : token.AccessToken;
        }

        public void SetUser(ClaimsPrincipal user)
        {
            if (_currentUser != user)
            {
                _currentUser = user;
            }
        }

        public async Task<UserMeResult> GetUserInfoAsync()
        {
            var userAuthId = (_currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                ?? throw new InvalidOperationException("User not authenticated");

            var userInfo = await _userAndTokenCache.GetUserInfoAsync(userAuthId);

            if (userInfo == null)
            {
                var token = await GetCurrentUserTokenAsync(userAuthId);
                var response = await _client.GetUserInfoAsync(token);
                var result = await response.Content.ReadFromJsonAsync<UserMeResult>();

                if (response.IsSuccessStatusCode && result != null)
                {
                    await _userAndTokenCache.SetUserInfoAsync(result);
                    return result;
                }
                else
                    throw new InvalidOperationException("Error getting user info");
            }
            else
                return userInfo;
        }
    }
    public sealed class UserCircuitHandler(
            AuthenticationStateProvider authenticationStateProvider,
            ClientUserService userService) : CircuitHandler, IDisposable
    {
        private readonly AuthenticationStateProvider authenticationStateProvider = authenticationStateProvider;
        private readonly ClientUserService userService = userService;

        public override Task OnCircuitOpenedAsync(Circuit circuit,
            CancellationToken cancellationToken)
        {
            authenticationStateProvider.AuthenticationStateChanged +=
                AuthenticationChanged;

            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }

        private void AuthenticationChanged(Task<AuthenticationState> task)
        {
            _ = UpdateAuthentication(task);

            async Task UpdateAuthentication(Task<AuthenticationState> task)
            {
                try
                {
                    var state = await task;
                    userService.SetUser(state.User);
                }
                catch
                {
                }
            }
        }

        public override async Task OnConnectionUpAsync(Circuit circuit,
            CancellationToken cancellationToken)
        {
            var state = await authenticationStateProvider.GetAuthenticationStateAsync();
            userService.SetUser(state.User);
        }

        public void Dispose()
        {
            authenticationStateProvider.AuthenticationStateChanged -=
                AuthenticationChanged;
        }
    }
}
