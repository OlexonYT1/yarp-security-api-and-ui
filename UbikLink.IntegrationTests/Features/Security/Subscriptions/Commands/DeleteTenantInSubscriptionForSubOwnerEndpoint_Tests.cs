using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Security.Api.Data.Init;

namespace UbikLink.IntegrationTests.Features.Security.Subscriptions.Commands
{
    public class DeleteTenantInSubscriptionForSubOwnerEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task DeleteTenantInSubscriptionForSubOwnerEndpoint_WithSubOwner_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            // Act
            var response = await AspireApp.AppHttpClient
                .DeleteAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantToDeleteId1}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTenantInSubscriptionForSubOwnerEndpoint_TenantNotFound_404()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var nonExistentTenantId = Guid.NewGuid();

            // Act
            var response = await AspireApp.AppHttpClient
                .DeleteAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{nonExistentTenantId}");
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("TENANT_NOT_FOUND", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task DeleteTenantInSubscriptionForSubOwnerEndpoint_TenantNotInSub_404()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            // Act
            var response = await AspireApp.AppHttpClient
                .DeleteAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantId4}");
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("TENANT_NOT_FOUND", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task DeleteTenantInSubscriptionForSubOwnerEndpoint_WithInactiveUser_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            // Act
            var response = await AspireApp.AppHttpClient
                .DeleteAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantToDeleteId2}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTenantInSubscriptionForSubOwnerEndpoint_WithMegaAdmin_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            // Act
            var response = await AspireApp.AppHttpClient
                .DeleteAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantToDeleteId2}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTenantInSubscriptionForSubOwnerEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await AspireApp.AppHttpClient
                .DeleteAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteTenantInSubscriptionForSubOwnerEndpoint_WithOtherTenant_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            // Act
            var response = await AspireApp.AppHttpClient
                .DeleteAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantToDeleteId2}");
            
            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
