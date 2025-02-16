using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Contracts.Tenants.Commands;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.IntegrationTests.Features.Security.Tenants.Commands
{
    public class AddTenantForAdminEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task AddTenantForAdminEndpoint_WithAdmin_Ok()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddTenantCommand
            {
                Label = "Test TenantX",
                SubscriptionId = TestData.SubscriptionId1
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/tenants", command);
            var result = await response.Content.ReadFromJsonAsync<TenantStandardResult>();

            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<TenantStandardResult>(result);
            Assert.Equal(command.Label, result.Label);
        }

        [Fact]
        public async Task AddTenantForAdminEndpoint_WithUser_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);
            var command = new AddTenantCommand
            {
                Label = "Test Tenant",
                SubscriptionId = TestData.SubscriptionId2
            };
            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/tenants", command);
            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddTenantForAdminEndpoint_WithUserInactive_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);
            var command = new AddTenantCommand
            {
                Label = "Test Tenant",
                SubscriptionId = TestData.SubscriptionId2
            };
            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/tenants", command);
            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddTenantForAdminEndpoint_WithOtherTenant_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);
            var command = new AddTenantCommand
            {
                Label = "Test Tenant",
                SubscriptionId = TestData.SubscriptionId2
            };
            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/tenants", command);
            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddTenantForAdminEndpoint_WithNoAuth_401()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;
            var command = new AddTenantCommand
            {
                Label = "Test Tenant",
                SubscriptionId = TestData.SubscriptionId2
            };
            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/tenants", command);
            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AddTenantForAdminEndpoint_WithBadSubscription_400ProblemDetails()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddTenantCommand
            {
                Label = "Test Tenant 2",
                SubscriptionId = NewId.NextGuid()
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/tenants", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("SUBSCRIPTION_FOR_TENANT_NOT_FOUND", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task AddTenantForAdminEndpoint_WithMaxTenantsLimit_400ProblemDetails()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddTenantCommand
            {
                Label = "Test Tenant 3",
                SubscriptionId = TestData.SubscriptionId2
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/tenants", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("MAX_TENANTS_LIMIT_FOR_SUBSCRIPTION", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task AddTenantForAdminEndpoint_WithEmptyLabel_400ProblemDetails()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddTenantCommand
            {
                Label = "", // Invalid label (empty)
                SubscriptionId = TestData.SubscriptionId1
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/tenants", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("Label is required.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task AddTenantForAdminEndpoint_WithLabelTooLong_400ProblemDetails()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddTenantCommand
            {
                Label = new string('A', 101), // Invalid label (too long)
                SubscriptionId = TestData.SubscriptionId1
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/tenants", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("Label must not exceed 100 characters.", result.Extensions["validationErrors"]!.ToString());
        }
    }
}
