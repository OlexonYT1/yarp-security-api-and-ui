using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Contracts.Authorizations.Results;

namespace UbikLink.IntegrationTests.Features.Security.Authorizations.Queries
{
    public class GetAuthorizationForAdminEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task GetAuthorizationForAdminEndpoint_WithAdmin_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var authorizationId = TestData.AuthorizationId1;

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/admin/authorizations/{authorizationId}");
            var result = await response.Content.ReadFromJsonAsync<AuthorizationStandardResult>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<AuthorizationStandardResult>(result);
        }

        [Fact]
        public async Task GetAuthorizationForAdminEndpoint_WithUser_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var authorizationId = TestData.AuthorizationId1;

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/admin/authorizations/{authorizationId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetAuthorizationForAdminEndpoint_WithUserInactive_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            var authorizationId = TestData.AuthorizationId1;

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/admin/authorizations/{authorizationId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetAuthorizationForAdminEndpoint_WithOtherTenant_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            var authorizationId = TestData.AuthorizationId1;

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/admin/authorizations/{authorizationId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetAuthorizationForAdminEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            var authorizationId = TestData.AuthorizationId1;

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/admin/authorizations/{authorizationId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAuthorizationForAdminEndpoint_WithNonExistentId_404ProblemDetails()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var authorizationId = Guid.NewGuid(); // Non-existent authorization ID

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/admin/authorizations/{authorizationId}");
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(404, result.Status);
            Assert.Contains("AUTHORIZATION_NOT_FOUND", result.Extensions["errors"]!.ToString());
        }
    }
}
