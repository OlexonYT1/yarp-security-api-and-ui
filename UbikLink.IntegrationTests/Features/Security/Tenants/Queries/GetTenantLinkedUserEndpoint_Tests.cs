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

namespace UbikLink.IntegrationTests.Features.Security.Tenants.Queries
{
    public class GetTenantLinkedUserEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task GetTenantLinkedUserEndpoint_WithAdmin_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            // Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/tenants/{TestData.TenantId1}/users/{TestData.UserId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetTenantLinkedUserEndpoint_WithUser_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            // Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/tenants/{TestData.TenantId1}/users/{TestData.UserId1}");
            
            var result = await response.Content.ReadFromJsonAsync<UserWithTenantRoleIdsResult>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<UserWithTenantRoleIdsResult>(result);
        }

        [Fact]
        public async Task GetTenantLinkedUserEndpoint_WithOtherTenant_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            // Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/tenants/{TestData.TenantId1}/users/{TestData.UserId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetTenantLinkedUserEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;
            // Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/tenants/{TestData.TenantId1}/users/{TestData.UserId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetTenantLinkedUserEndpoint_WithInvalidTenantId_404()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            // Act
            // Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/tenants/{Guid.NewGuid()}/users/{TestData.UserId1}");

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("TENANT_NOT_FOUND", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task GetTenantLinkedUserEndpoint_WithInvalidUserId_404()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            // Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/tenants/{TestData.TenantId1}/users/{Guid.NewGuid()}");

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("USER_NOT_FOUND", result.Extensions["errors"]!.ToString());
        }


    }
}
