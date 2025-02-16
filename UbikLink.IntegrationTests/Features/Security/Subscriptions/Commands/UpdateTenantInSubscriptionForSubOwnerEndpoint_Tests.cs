using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Contracts.Subscriptions.Commands;
using UbikLink.Security.Contracts.Tenants.Results;

namespace UbikLink.IntegrationTests.Features.Security.Subscriptions.Commands
{
    public class UpdateTenantInSubscriptionForSubOwnerEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task UpdateTenantInSubscriptionForSubOwnerEndpoint_WithSubOwner_Ok()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxx updated",
                LinkedUsersIds = [TestData.UserId3, TestData.UserId1],
                SubscriptionId = TestData.SubscriptionId1,
                Version = TestData.TenantId1,
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantId1}", command);
            var result = await response.Content.ReadFromJsonAsync<TenantSubOwnerResult>();

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<TenantSubOwnerResult>(result);
            Assert.Equal(command.Label, result.Label);
            Assert.Equal(command.LinkedUsersIds.OrderBy(x => x), result.LinkedUserIds.OrderBy(x => x));
        }

        [Fact]
        public async Task UpdateTenantInSubscriptionForSubOwnerEndpoint_WithBadVersion_Ok()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxx updated",
                LinkedUsersIds = [TestData.UserId3, TestData.UserId1],
                SubscriptionId = TestData.SubscriptionId1,
                Version = TestData.TenantId1,
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantId3}", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("DB_CONCURRENCY_CONFLICT", result.Extensions["errors"]!.ToString());

        }

        [Fact]
        public async Task UpdateTenantInSubscriptionForSubOwnerEndpoint__WithUser_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            var command = new UpdateSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxx updatesd",
                LinkedUsersIds = [TestData.UserId3, TestData.UserId1],
                SubscriptionId = TestData.SubscriptionId1,
                Version = TestData.TenantId1,
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantId1}", command);

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTenantInSubscriptionForSubOwnerEndpoint__WithInactiveUser_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            var command = new UpdateSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxx updated",
                LinkedUsersIds = [TestData.UserId3, TestData.UserId1],
                SubscriptionId = TestData.SubscriptionId1,
                Version = TestData.TenantId1,
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantId1}", command);

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTenantInSubscriptionForSubOwnerEndpoint__WithMegaAdmin_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxx updated",
                LinkedUsersIds = [TestData.UserId3, TestData.UserId1],
                SubscriptionId = TestData.SubscriptionId1,
                Version = TestData.TenantId1,
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantId1}", command);

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTenantInSubscriptionForSubOwnerEndpoint__WithNoAuth_401()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            var command = new UpdateSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxx updated",
                LinkedUsersIds = [TestData.UserId3, TestData.UserId1],
                SubscriptionId = Guid.NewGuid(),
                Version = TestData.TenantId1,
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantId1}", command);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTenantInSubscriptionForSubOwnerEndpoint_WithInvalidLabel_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateSubscriptionLinkedTenantCommand
            {
                Label = "", // Invalid label
                LinkedUsersIds = [TestData.UserId3, TestData.UserId1],
                SubscriptionId = TestData.SubscriptionId1,
                Version = TestData.TenantId1,
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantId1}", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("Label is required.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task UpdateTenantInSubscriptionForSubOwnerEndpoint_WithTooLongLabel_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateSubscriptionLinkedTenantCommand
            {
                Label = new string('a', 101), // Label exceeds 100 characters
                LinkedUsersIds = [TestData.UserId3, TestData.UserId1 ],
                SubscriptionId = TestData.SubscriptionId1,
                Version = TestData.TenantId3,
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantId3}", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("Label must not exceed 100 characters.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task UpdateTenantInSubscriptionForSubOwnerEndpoint_WithEmptySubscriptionId_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenant",
                LinkedUsersIds = [TestData.UserId3, TestData.UserId1],
                SubscriptionId = Guid.Empty, // Invalid SubscriptionId
                Version = TestData.TenantId3,
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantId3}", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("SubscriptionId is required.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task UpdateTenantInSubscriptionForSubOwnerEndpoint_TenantNotFound_404()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var nonExistentTenantId = Guid.NewGuid();

            var command = new UpdateSubscriptionLinkedTenantCommand
            {
                Label = "Non-existent Tenant",
                LinkedUsersIds = [TestData.UserId3],
                SubscriptionId = TestData.SubscriptionId1,
                Version = nonExistentTenantId,
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{nonExistentTenantId}", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("TENANT_NOT_FOUND", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task UpdateTenantInSubscriptionForSubOwnerEndpoint_UserNotInSubscription_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenant",
                LinkedUsersIds = [TestData.UserId2], // User not in subscription
                SubscriptionId = TestData.SubscriptionId1,
                Version = TestData.TenantId1,
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantId1}", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("USER_ID_NOT_VALID_FOR_THIS_SUBSCRIPTION", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task UpdateTenantInSubscriptionForSubOwnerEndpoint_UserNotSelectedSub_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenant",
                LinkedUsersIds = [TestData.UserId1],
                SubscriptionId = TestData.SubscriptionId3,
                Version = TestData.TenantId3,
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantId3}", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("TENANT_NOT_FOUND", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task UpdateTenantInSubscriptionForSubOwnerEndpoint_WithUserNotInSubscription_400()
        {
            // Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxax",
                LinkedUsersIds = [TestData.UserId2],
                SubscriptionId = TestData.SubscriptionId1,
                Version = TestData.TenantId1,
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/tenants/{TestData.TenantId1}", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("USER_ID_NOT_VALID_FOR_THIS_SUBSCRIPTION", result.Extensions["errors"]!.ToString());

        }
    }
}
