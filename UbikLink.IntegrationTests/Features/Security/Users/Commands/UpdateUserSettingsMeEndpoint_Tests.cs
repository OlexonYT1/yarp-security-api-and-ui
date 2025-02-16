using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Contracts.Users.Commands;

namespace UbikLink.IntegrationTests.Features.Security.Users.Commands
{
    public class UpdateUserSettingsMeEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task UpdateUserSettingsMeEndpoint_WithAdmin_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new SetSettingsUserMeCommand
            {
                TenantId = TestData.TenantId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/me/selecttenant", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("USER_BAD_TENANT_LINK", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task UpdateUserSettingsMeEndpoint_WithOtherTenant_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            var command = new SetSettingsUserMeCommand
            {
                TenantId = TestData.TenantId2
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/me/selecttenant", command);

            var result = await response.Content.ReadFromJsonAsync<Guid?>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(command.TenantId, result);
        }

        [Fact]
        public async Task UpdateUserSettingsMeEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            var command = new SetSettingsUserMeCommand
            {
                TenantId = TestData.TenantId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/me/selecttenant", command);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserSettingsMeEndpoint_WithInvalidTenantId_400ProblemDetails()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new SetSettingsUserMeCommand
            {
                TenantId = Guid.Empty // Invalid tenant ID
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/me/selecttenant", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("USER_BAD_TENANT_LINK", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task UpdateUserSettingsMeEndpoint_WithNonExistentTenantId_404ProblemDetails()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new SetSettingsUserMeCommand
            {
                TenantId = Guid.NewGuid() // Non-existent tenant ID
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/me/selecttenant", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("USER_BAD_TENANT_LINK", result.Extensions["errors"]!.ToString());
        }


        [Fact]
        public async Task UpdateUserSettingsMeEndpoint_WithTenantNotLinked_404ProblemDetails()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            var command = new SetSettingsUserMeCommand
            {
                TenantId = TestData.TenantId1// Non-existent tenant ID
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/me/selecttenant", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("USER_BAD_TENANT_LINK", result.Extensions["errors"]!.ToString());
        }
    }
}
