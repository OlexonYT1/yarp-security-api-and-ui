using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.IntegrationTests.Features.Security.Tenants.Queries
{
    public class GetAllTenantsForAdminEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task GetAllTenantsForAdminEndpoint_WithAdmin_Ok()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            //Act
            var response = await AspireApp.AppHttpClient.GetAsync("/security/api/v1/admin/tenants/");
            var result = await response.Content.ReadFromJsonAsync<List<TenantStandardResult>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<List<TenantStandardResult>>(result);
        }

        [Fact]
        public async Task GetAllTenantsForAdminEndpoint_WithUser_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            //Act
            var response = await AspireApp.AppHttpClient.GetAsync("/security/api/v1/admin/tenants/");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetAllTenantsForAdminEndpoint_WithOtherTenant_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            //Act
            var response = await AspireApp.AppHttpClient.GetAsync("/security/api/v1/admin/tenants/");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetAllTenantsForAdminEndpoint_WithUserInactive_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            //Act
            var response = await AspireApp.AppHttpClient.GetAsync("/security/api/v1/admin/tenants/");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }


        [Fact]
        public async Task GetAllTenantsForAdminEndpoint_WithNoAuth_401()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            //Act
            var response = await AspireApp.AppHttpClient.GetAsync("/security/api/v1/admin/tenants/");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
