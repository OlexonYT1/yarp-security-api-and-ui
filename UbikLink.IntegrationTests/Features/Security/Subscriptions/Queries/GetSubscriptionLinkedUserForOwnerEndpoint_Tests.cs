using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.IntegrationTests.Features.Security.Subscriptions.Queries
{
    public class GetSubscriptionLinkedUserForOwnerEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task GetSubscriptionLinkedUserForOwnerEndpoint_WithUser_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/subowner/subscriptions/selected/users/{TestData.UserId1}");
            var result = await response.Content.ReadFromJsonAsync<UserWithInfoSubOwnerResult>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<UserWithInfoSubOwnerResult>(result);
        }

        [Fact]
        public async Task GetSubscriptionLinkedUserForOwnerEndpoint_WithAdmin_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/subowner/subscriptions/selected/users/{TestData.UserId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetSubscriptionLinkedUserForOwnerEndpoint_WithUserInactive_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/subowner/subscriptions/selected/users/{TestData.UserId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetSubscriptionLinkedUserForOwnerEndpoint_WithOtherTenant_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/subowner/subscriptions/selected/users/{TestData.UserId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetSubscriptionLinkedUserForOwnerEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/subowner/subscriptions/selected/users/{TestData.UserId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Fact]
        public async Task GetSubscriptionLinkedUserForOwnerEndpoint_UserNotFound_404()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var nonExistentUserId = Guid.NewGuid();

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/subowner/subscriptions/selected/users/{nonExistentUserId}");
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("USER_NOT_FOUND", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task GetSubscriptionLinkedUserForOwnerEndpoint_UserNotInSubscription_404()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/subowner/subscriptions/selected/users/{TestData.UserId2}");
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("USER_NOT_FOUND", result.Extensions["errors"]!.ToString());
        }
    }
}
