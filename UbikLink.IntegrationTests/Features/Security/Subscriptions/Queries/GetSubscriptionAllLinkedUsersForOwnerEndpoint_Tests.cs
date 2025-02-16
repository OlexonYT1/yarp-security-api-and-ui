using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.IntegrationTests.Features.Security.Subscriptions.Queries
{
    public class GetSubscriptionAllLinkedUsersForOwnerEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task GetSubscriptionLinkedUsersForOwnerEndpoint_WithAdmin_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/subowner/subscriptions/selected/users");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetSubscriptionLinkedUsersForOwnerEndpoint_WithUser_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/subowner/subscriptions/selected/users");
            var result = await response.Content.ReadFromJsonAsync<List<UserSubOwnerResult>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<List<UserSubOwnerResult>>(result);
        }

        [Fact]
        public async Task GetSubscriptionLinkedUsersForOwnerEndpoint_WithUserInactive_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/subowner/subscriptions/selected/users");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetSubscriptionLinkedUsersForOwnerEndpoint_WithNoAuth_401()
        {
            //Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/subowner/subscriptions/selected/users");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetSubscriptionLinkedUsersForOwnerEndpoint_WithOtherTenantNotSubOwner_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/subowner/subscriptions/selected/users");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
