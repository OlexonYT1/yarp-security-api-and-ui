using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Contracts.Authorizations.Commands;

namespace UbikLink.IntegrationTests.Features.Security.Authorizations.Commands
{
    public class BatchDeleteAuthorizationForAdminEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task BatchDeleteAuthorizationForAdminEndpoint_WithAdmin_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new BatchDeleteAuthorizationCommand
            {
                AuthorizationIds = [TestData.AuthorizationIdToDel2, TestData.AuthorizationIdToDel3]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations/commands/batch-delete", command);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task BatchDeleteAuthorizationForAdminEndpoint_WithUser_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new BatchDeleteAuthorizationCommand
            {
                AuthorizationIds = [TestData.AuthorizationIdToDel2, TestData.AuthorizationIdToDel3]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations/commands/batch-delete", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task BatchDeleteAuthorizationForAdminEndpoint_WithUserInactive_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            var command = new BatchDeleteAuthorizationCommand
            {
                AuthorizationIds = [TestData.AuthorizationIdToDel2, TestData.AuthorizationIdToDel3]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations/commands/batch-delete", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task BatchDeleteAuthorizationForAdminEndpoint_WithOtherTenant_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            var command = new BatchDeleteAuthorizationCommand
            {
                AuthorizationIds = [TestData.AuthorizationIdToDel2, TestData.AuthorizationIdToDel3]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations/commands/batch-delete", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task BatchDeleteAuthorizationForAdminEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            var command = new BatchDeleteAuthorizationCommand
            {
                AuthorizationIds = [TestData.AuthorizationIdToDel2, TestData.AuthorizationIdToDel3]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations/commands/batch-delete", command);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task BatchDeleteAuthorizationForAdminEndpoint_WithNonExistentId_404ProblemDetails()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new BatchDeleteAuthorizationCommand
            {
                AuthorizationIds = [Guid.NewGuid()]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations/commands/batch-delete", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("AUTHORIZATION_BAD_IDS", result.Extensions["errors"]!.ToString()!);
        }

        [Fact]
        public async Task BatchDeleteAuthorizationForAdminEndpoint_WithPartialNonExistentIds_400ProblemDetails()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new BatchDeleteAuthorizationCommand
            {
                AuthorizationIds = [TestData.AuthorizationId2, Guid.NewGuid()]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations/commands/batch-delete", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("AUTHORIZATION_BAD_IDS", result.Extensions["errors"]!.ToString()!);
        }
    }
}
