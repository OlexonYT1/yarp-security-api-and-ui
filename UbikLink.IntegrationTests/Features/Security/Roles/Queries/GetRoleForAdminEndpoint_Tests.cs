using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Api.Features.Roles.Services.Poco;

namespace UbikLink.IntegrationTests.Features.Security.Roles.Queries
{
    public class GetRoleForAdminEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task GetRoleForAdminEndpoint_WithAdmin_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/admin/roles/{TestData.RoleId1}");
            var test = await response.Content.ReadAsStringAsync();
            var result = await response.Content.ReadFromJsonAsync<RoleWithAuthorizationIds>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<RoleWithAuthorizationIds>(result);
        }

        [Fact]
        public async Task GetRoleForAdminEndpoint_WithUser_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/admin/roles/{TestData.RoleId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetRoleForAdminEndpoint_WithUserInactive_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/admin/roles/{TestData.RoleId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetRoleForAdminEndpoint_WithOtherTenant_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/admin/roles/{TestData.RoleId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetRoleForAdminEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/admin/roles/{TestData.RoleId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetRoleForAdminEndpoint_WithInvalidRoleId_404()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync($"/security/api/v1/admin/roles/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
