using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Api.Data.Init;

namespace UbikLink.IntegrationTests.Features.Security.Authorizations.Commands
{
    public class DeleteAuthorizationForAdminEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task DeleteAuthorizationForAdminEndpoint_WithAdmin_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var authorizationId = TestData.AuthorizationIdToDel;
            // Act
            var response = await AspireApp.AppHttpClient
                .DeleteAsync($"/security/api/v1/admin/authorizations/{authorizationId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAuthorizationForAdminEndpoint_WithUser_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var authorizationId = TestData.AuthorizationIdToDel;

            // Act
            var response = await AspireApp.AppHttpClient
                .DeleteAsync($"/security/api/v1/admin/authorizations/{authorizationId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAuthorizationForAdminEndpoint_WithUserInactive_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            var authorizationId = TestData.AuthorizationIdToDel;

            // Act
            var response = await AspireApp.AppHttpClient
                .DeleteAsync($"/security/api/v1/admin/authorizations/{authorizationId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAuthorizationForAdminEndpoint_WithOtherTenant_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            var authorizationId = TestData.AuthorizationIdToDel;

            // Act
            var response = await AspireApp.AppHttpClient
                .DeleteAsync($"/security/api/v1/admin/authorizations/{authorizationId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAuthorizationForAdminEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            var authorizationId = TestData.AuthorizationIdToDel;

            // Act
            var response = await AspireApp.AppHttpClient
                .DeleteAsync($"/security/api/v1/admin/authorizations/{authorizationId}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAuthorizationForAdminEndpoint_WithNonExistentId_404ProblemDetails()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var authorizationId = Guid.NewGuid(); // Non-existent authorization ID

            // Act
            var response = await AspireApp.AppHttpClient
                .DeleteAsync($"/security/api/v1/admin/authorizations/{authorizationId}");

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(404, result.Status);
            Assert.Contains("AUTHORIZATION_NOT_FOUND", result.Extensions["errors"]!.ToString()!);
        }
    }
}
