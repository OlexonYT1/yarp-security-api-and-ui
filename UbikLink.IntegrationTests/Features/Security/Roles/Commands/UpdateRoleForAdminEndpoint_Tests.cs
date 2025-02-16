using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Api.Features.Roles.Services.Poco;
using UbikLink.Security.Contracts.Roles.Commands;
using UbikLink.Security.Contracts.Roles.Results;

namespace UbikLink.IntegrationTests.Features.Security.Roles.Commands
{
    public class UpdateRoleForAdminEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task UpdateRoleForAdminEndpoint_WithAdmin_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateRoleCommand
            {
                Code = "UPDATED_ROLE",
                Description = "Updated Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2],
                Version = TestData.RoleId2
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/roles/{TestData.RoleId2}", command);

            var result = await response.Content.ReadFromJsonAsync<RoleAdminResult>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<RoleAdminResult>(result);
            Assert.Equal(command.Code, result.Code);
            Assert.Equal(command.Description, result.Description);
            Assert.Equal(command.AuthorizationIds.OrderBy(x => x), result.AuthorizationIds.OrderBy(x => x));
        }

        [Fact]
        public async Task UpdateRoleForAdminEndpoint_WithUser_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateRoleCommand
            {
                Code = "UPDATED_ROLE",
                Description = "Updated Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2],
                Version = TestData.RoleId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/roles/{TestData.RoleId1}", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRoleForAdminEndpoint_WithUserInactive_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            var command = new UpdateRoleCommand
            {
                Code = "UPDATED_ROLE",
                Description = "Updated Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2],
                Version = TestData.RoleId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/roles/{TestData.RoleId1}", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRoleForAdminEndpoint_WithOtherTenant_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            var command = new UpdateRoleCommand
            {
                Code = "UPDATED_ROLE",
                Description = "Updated Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2],
                Version = TestData.RoleId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/roles/{TestData.RoleId1}", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRoleForAdminEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            var command = new UpdateRoleCommand
            {
                Code = "UPDATED_ROLE",
                Description = "Updated Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2],
                Version = TestData.RoleId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/roles/{TestData.RoleId1}", command);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRoleForAdminEndpoint_WithInvalidRoleId_404()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateRoleCommand
            {
                Code = "UPDATED_ROLE",
                Description = "Updated Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2],
                Version = TestData.RoleId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/roles/{Guid.NewGuid()}", command);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRoleForAdminEndpoint_WithExistingCode_409()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateRoleCommand
            {
                Code = "TenantViewer",
                Description = "Updated Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2],
                Version = TestData.RoleId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/roles/{TestData.RoleId1}", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("ROLE_ALREADY_EXISTS", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task UpdateRoleForAdminEndpoint_WithEmptyCode_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateRoleCommand
            {
                Code = "", // Invalid code (empty)
                Description = "Updated Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2],
                Version = TestData.RoleId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/roles/{TestData.RoleId1}", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("Code is required.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task UpdateRoleForAdminEndpoint_WithCodeTooLong_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateRoleCommand
            {
                Code = new string('A', 51), // Invalid code (too long)
                Description = "Updated Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2],
                Version = TestData.RoleId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/roles/{TestData.RoleId1}", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("Code must not exceed 50 characters.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task UpdateRoleForAdminEndpoint_WithDescriptionTooLong_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateRoleCommand
            {
                Code = "UPDATED_ROLE",
                Description = new string('A', 501), // Invalid description (too long)
                AuthorizationIds = [TestData.AuthorizationId1, TestData.AuthorizationId2],
                Version = TestData.RoleId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/roles/{TestData.RoleId1}", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("Description must not exceed 500 characters.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task UpdateRoleForAdminEndpoint_WithNonExistentAuthorizationId_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateRoleCommand
            {
                Code = "UPDATED_ROLE_TEST",
                Description = "Updated Role Description",
                AuthorizationIds = [TestData.AuthorizationId1, Guid.NewGuid()], // Non-existent authorization ID
                Version = TestData.RoleId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/roles/{TestData.RoleId1}", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("AUTHORIZATION_BAD_IDS", result.Extensions["errors"]!.ToString());
        }
    }
}
