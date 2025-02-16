using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.IntegrationTests.Features.Security.Tenants.Queries
{
    public class GetTenantForAdminEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task GetTenantForAdminEndpoint_WithAdmin_Ok()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            //Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/admin/tenants/{TestData.TenantId1}");

            var result = await response.Content.ReadFromJsonAsync<TenantStandardResult>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<TenantStandardResult>(result);
        }

        [Fact]
        public async Task GetTenantForAdminEndpoint_WithUser_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            //Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/admin/tenants/{TestData.TenantId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetTenantForAdminEndpoint_WithUserInactive_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            //Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/admin/tenants/{TestData.TenantId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetTenantForAdminEndpoint_WithOtherTenant_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            //Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/admin/tenants/{TestData.TenantId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetTenantForAdminEndpoint_WithNoAuth_401()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            //Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/admin/tenants/{TestData.TenantId1}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetTenantForAdminEndpoint_WithBadId_404ProblemDetails()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            //Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/admin/tenants/{NewId.NextGuid()}");

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(404, result.Status);
            Assert.Contains("TENANT_NOT_FOUND", result.Extensions["errors"]!.ToString());
        }
    }
}

