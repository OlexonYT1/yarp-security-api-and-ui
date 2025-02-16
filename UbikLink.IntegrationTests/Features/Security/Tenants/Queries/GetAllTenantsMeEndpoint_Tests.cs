using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.IntegrationTests.Features.Security.Tenants.Queries
{
    public class GetAllTenantsMeEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task GetAllTenantsMeEndpoint_WithAdmin_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync("/security/api/v1/me/tenants");
            var result = await response.Content.ReadFromJsonAsync<List<TenantStandardResult>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<List<TenantStandardResult>>(result);
        }

        [Fact]
        public async Task GetAllTenantsMeEndpoint_WithUser_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync("/security/api/v1/me/tenants");
            var result = await response.Content.ReadFromJsonAsync<List<TenantStandardResult>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<List<TenantStandardResult>>(result);
            Assert.Contains(result, tenant => tenant.Id == TestData.TenantId1);
        }

        [Fact]
        public async Task GetAllTenantsMeEndpoint_WithOtherTenant_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync("/security/api/v1/me/tenants");
            var result = await response.Content.ReadFromJsonAsync<List<TenantStandardResult>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<List<TenantStandardResult>>(result);
            Assert.Contains(result, tenant => tenant.Id == TestData.TenantId2);
        }

        [Fact]
        public async Task GetAllTenantsMeEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync("/security/api/v1/me/tenants");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
