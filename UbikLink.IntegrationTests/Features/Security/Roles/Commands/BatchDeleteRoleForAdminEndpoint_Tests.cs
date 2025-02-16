using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Contracts.Roles.Commands;

namespace UbikLink.IntegrationTests.Features.Security.Roles.Commands
{
    public class BatchDeleteRoleForAdminEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task BatchDeleteRoleForAdminEndpoint_WithAdmin_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new BatchDeleteRoleCommand
            {
                RoleIds = [TestData.RoleIdToDel1, TestData.RoleIdToDel2]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles/commands/batch-delete", command);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task BatchDeleteRoleForAdminEndpoint_WithUser_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new BatchDeleteRoleCommand
            {
                RoleIds = [TestData.RoleIdToDel1, TestData.RoleIdToDel2]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles/commands/batch-delete", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task BatchDeleteRoleForAdminEndpoint_WithUserInactive_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            var command = new BatchDeleteRoleCommand
            {
                RoleIds = [TestData.RoleIdToDel1, TestData.RoleIdToDel2]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles/commands/batch-delete", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task BatchDeleteRoleForAdminEndpoint_WithOtherTenant_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            var command = new BatchDeleteRoleCommand
            {
                RoleIds = [TestData.RoleIdToDel1, TestData.RoleIdToDel2]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles/commands/batch-delete", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task BatchDeleteRoleForAdminEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            var command = new BatchDeleteRoleCommand
            {
                RoleIds = [TestData.RoleIdToDel1, TestData.RoleIdToDel2]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles/commands/batch-delete", command);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task BatchDeleteRoleForAdminEndpoint_WithNonExistentId_404ProblemDetails()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new BatchDeleteRoleCommand
            {
                RoleIds = [Guid.NewGuid()]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles/commands/batch-delete", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("ROLE_BAD_IDS", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task BatchDeleteRoleForAdminEndpoint_WithPartialNonExistentIds_400ProblemDetails()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new BatchDeleteRoleCommand
            {
                RoleIds = [TestData.RoleIdToDel1, Guid.NewGuid()]
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/roles/commands/batch-delete", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("ROLE_BAD_IDS", result.Extensions["errors"]!.ToString());
        }
    }
}
