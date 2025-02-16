using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.IntegrationTests.Features.Security.Tenants.Queries
{
    public class GetTenantEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task GetTenantEndpoint_WithAdmin_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/tenants/{TestData.TenantId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetTenantEndpoint_WithUser_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/tenants/{TestData.TenantId1}");
            var result = await response.Content.ReadFromJsonAsync<TenantStandardResult>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<TenantStandardResult>(result);
        }

        [Fact]
        public async Task GetTenantEndpoint_WithUserInactive_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/tenants/{TestData.TenantId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetTenantEndpoint_WithOtherTenant_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/tenants/{TestData.TenantId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetTenantEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/tenants/{TestData.TenantId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
