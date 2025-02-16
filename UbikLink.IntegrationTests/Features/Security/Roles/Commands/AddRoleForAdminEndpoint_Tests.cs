using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Api.Features.Roles.Services.Poco;
using UbikLink.Security.Contracts.Roles.Commands;

namespace UbikLink.IntegrationTests.Features.Security.Roles.Commands
{
    public class AddRoleForAdminEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task AddRoleForAdminEndpoint_WithAdmin_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddRoleCommand
            {
                Code = "NEW_ROLE",
                Description = "New Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles", command);

            var result = await response.Content.ReadFromJsonAsync<RoleWithAuthorizationIds>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<RoleWithAuthorizationIds>(result);
            Assert.Equal(command.Code, result.Code);
            Assert.Equal(command.Description, result.Description);
            Assert.Equal(command.AuthorizationIds.OrderBy(x => x), result.AuthorizationIds.OrderBy(x => x));
        }

        [Fact]
        public async Task AddRoleForAdminEndpoint_WithUser_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new AddRoleCommand
            {
                Code = "NEW_ROLE",
                Description = "New Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddRoleForAdminEndpoint_WithUserInactive_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            var command = new AddRoleCommand
            {
                Code = "NEW_ROLE",
                Description = "New Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddRoleForAdminEndpoint_WithOtherTenant_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            var command = new AddRoleCommand
            {
                Code = "NEW_ROLE",
                Description = "New Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddRoleForAdminEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            var command = new AddRoleCommand
            {
                Code = "NEW_ROLE",
                Description = "New Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles", command);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AddRoleForAdminEndpoint_WithExistingCode_409()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddRoleCommand
            {
                Code = "TenantViewer",
                Description = "New Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("ROLE_ALREADY_EXISTS", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task AddRoleForAdminEndpoint_WithEmptyCode_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddRoleCommand
            {
                Code = "", // Invalid code (empty)
                Description = "New Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("Code is required.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task AddRoleForAdminEndpoint_WithCodeTooLong_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddRoleCommand
            {
                Code = new string('A', 51), // Invalid code (too long)
                Description = "New Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("Code must not exceed 50 characters.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task AddRoleForAdminEndpoint_WithDescriptionTooLong_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddRoleCommand
            {
                Code = "NEW_ROLE",
                Description = new string('A', 501), // Invalid description (too long)
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("Description must not exceed 500 characters.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task AddRoleForAdminEndpoint_WithNonExistentAuthorizationId_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddRoleCommand
            {
                Code = "NEW_ROLE_TEST",
                Description = "New Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, Guid.NewGuid()] // Non-existent authorization ID
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("AUTHORIZATION_BAD_IDS", result.Extensions["errors"]!.ToString());
        }
    }
}
