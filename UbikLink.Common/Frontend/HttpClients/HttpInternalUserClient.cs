using Microsoft.Extensions.Caching.Hybrid;
using System.Net.Http;
using System.Net.Http.Json;
using UbikLink.Common.Frontend.Auth;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.Common.Frontend.HttpClients
{
    public class HttpInternalUserClient(HttpClient client) : IHttpUserClient
    {
        private readonly HttpClient _client = client;

        public async Task<HttpResponseMessage> GetUserInfoAsync(string token)
        {
            SetSecruityHeader(token);
            return await _client.GetAsync("authinfo");
        }

        private void SetSecruityHeader(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ApplicationException("Cannot retrieve info, refresh your app");

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
    }
}
