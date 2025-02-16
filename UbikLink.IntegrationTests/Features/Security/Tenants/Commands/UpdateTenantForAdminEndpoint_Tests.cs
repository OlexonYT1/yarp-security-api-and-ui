using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Contracts.Tenants.Commands;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.IntegrationTests.Features.Security.Tenants.Commands
{
    public class UpdateTenantForAdminEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task UpdateTenantForAdminEndpoint_WithAdmin_Ok()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateTenantCommand
            {
                Label = "Updated Tenant",
                SubscriptionId = TestData.SubscriptionId2,
                IsActivated = true,
                Version = TestData.TenantId2
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/tenants/{TestData.TenantId2}", command);

            var result = await response.Content.ReadFromJsonAsync<TenantStandardResult>();

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<TenantStandardResult>(result);
            Assert.Equal(command.Label, result.Label);
        }

        [Fact]
        public async Task UpdateTenantForAdminEndpoint_WithUser_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);
            var command = new UpdateTenantCommand
            {
                Label = "Updated Tenant",
                SubscriptionId = TestData.SubscriptionId1,
                IsActivated = true,
                Version = TestData.TenantId1
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/tenants/{TestData.TenantId1}", command);

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTenantForAdminEndpoint_WithUserInactive_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);
            var command = new UpdateTenantCommand
            {
                Label = "Updated Tenant",
                SubscriptionId = TestData.SubscriptionId1,
                IsActivated = true,
                Version = TestData.TenantId1
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/tenants/{TestData.TenantId1}", command);

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTenantForAdminEndpoint_WithOtherTenant_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            var command = new UpdateTenantCommand
            {
                Label = "Updated Tenant",
                SubscriptionId = TestData.SubscriptionId1,
                IsActivated = true,
                Version = TestData.TenantId1
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/tenants/{TestData.TenantId1}", command);
            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTenantForAdminEndpoint_WithNoAuth_401()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;
            var command = new UpdateTenantCommand
            {
                Label = "Updated Tenant",
                SubscriptionId = TestData.SubscriptionId1,
                IsActivated = true,
                Version = TestData.TenantId1
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/tenants/{TestData.TenantId1}", command);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTenantForAdminEndpoint_WithBadSubscription_400ProblemDetails()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateTenantCommand
            {
                Label = "Updated Tenant",
                SubscriptionId = NewId.NextGuid(),
                IsActivated = true,
                Version = TestData.TenantId1
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/tenants/{TestData.TenantId1}", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("SUBSCRIPTION_FOR_TENANT_NOT_FOUND", result.Extensions["errors"]!.ToString()!);
        }

        [Fact]
        public async Task UpdateTenantForAdminEndpoint_WithBadVersion_409ProblemDetails()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateTenantCommand
            {
                Label = "Updated Tenant",
                SubscriptionId = TestData.SubscriptionId1,
                IsActivated = true,
                Version = NewId.NextGuid()
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/tenants/{TestData.TenantId1}", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(409, result.Status);
            Assert.Contains("DB_CONCURRENCY_CONFLICT", result.Extensions["errors"]!.ToString()!);
        }

        [Fact]
        public async Task UpdateTenantForAdminEndpoint_WithEmptyLabel_400ProblemDetails()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateTenantCommand
            {
                Label = "", // Invalid label (empty)
                SubscriptionId = TestData.SubscriptionId1,
                IsActivated = true,
                Version = TestData.TenantId1
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/tenants/{TestData.TenantId1}", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("Label is required.", result.Extensions["validationErrors"]!.ToString()!);
        }

        [Fact]
        public async Task UpdateTenantForAdminEndpoint_WithLabelTooLong_400ProblemDetails()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateTenantCommand
            {
                Label = new string('A', 101), // Invalid label (too long)
                SubscriptionId = TestData.SubscriptionId1,
                IsActivated = true,
                Version = TestData.TenantId1
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/tenants/{TestData.TenantId1}", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("Label must not exceed 100 characters.", result.Extensions["validationErrors"]!.ToString()!);
        }
    }
}
