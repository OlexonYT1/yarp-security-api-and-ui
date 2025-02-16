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
    public class AddTenantInSubscriptionForSubOwnerEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task AddTenantInSubscriptionForSubOwnerEndpoint_WithSubOwner_Ok()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new AddSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxx",
                LinkedUsersIds = [TestData.UserId3],
                SubscriptionId = TestData.SubscriptionId1,
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/subowner/subscriptions/selected/tenants", command);
            var result = await response.Content.ReadFromJsonAsync<TenantSubOwnerResult>();

            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<TenantSubOwnerResult>(result);
            Assert.Equal(command.Label, result.Label);
            Assert.Equal(command.LinkedUsersIds, result.LinkedUserIds);
        }

        [Fact]
        public async Task AddTenantInSubscriptionForSubOwnerEndpoint_WithUser_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);
            var command = new AddSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxx",
                LinkedUsersIds = [TestData.UserId3],
                SubscriptionId = TestData.SubscriptionId1,
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/subowner/subscriptions/selected/tenants", command);

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddTenantInSubscriptionForSubOwnerEndpoint_WithUserInactive_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);
            var command = new AddSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxx",
                LinkedUsersIds = [TestData.UserId3],
                SubscriptionId = TestData.SubscriptionId1,
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/subowner/subscriptions/selected/tenants", command);

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddTenantInSubscriptionForSubOwnerEndpoint_WithOtherTenant_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);
            var command = new AddSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxx",
                LinkedUsersIds = [TestData.UserId3],
                SubscriptionId = TestData.SubscriptionId1,
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/subowner/subscriptions/selected/tenants", command);

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddTenantInSubscriptionForSubOwnerEndpoint_WithAdmin_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);
            var command = new AddSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxx",
                LinkedUsersIds = [TestData.UserId3],
                SubscriptionId = TestData.SubscriptionId1,
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/subowner/subscriptions/selected/tenants", command);

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddTenantInSubscriptionForSubOwnerEndpoint_WithNoAuth_401()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            var command = new AddSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxx",
                LinkedUsersIds = [TestData.UserId3],
                SubscriptionId = TestData.SubscriptionId1,
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/subowner/subscriptions/selected/tenants", command);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AddTenantInSubscriptionForSubOwnerEndpoint_WithInvalidLabel_400()
        {
            // Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new AddSubscriptionLinkedTenantCommand
            {
                Label = "", // Invalid label
                LinkedUsersIds = [TestData.UserId3],
                SubscriptionId = TestData.SubscriptionId1,
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/subowner/subscriptions/selected/tenants", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("Label is required.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task AddTenantInSubscriptionForSubOwnerEndpoint_WithInvalidSubscriptionId_400()
        {
            // Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new AddSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxx",
                LinkedUsersIds = [TestData.UserId3],
                SubscriptionId = Guid.Empty, // Invalid subscriptions ID
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/subowner/subscriptions/selected/tenants", command);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AddTenantInSubscriptionForSubOwnerEndpoint_WithUserNotInSubscription_400()
        {
            // Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new AddSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxax",
                LinkedUsersIds = [TestData.UserId2],
                SubscriptionId = TestData.SubscriptionId1,
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/subowner/subscriptions/selected/tenants", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("USER_ID_NOT_VALID_FOR_THIS_SUBSCRIPTION", result.Extensions["errors"]!.ToString());
            
        }

        [Fact]
        public async Task AddTenantInSubscriptionForSubOwnerEndpoint_WithNotSelectedSubscription_400()
        {
            // Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new AddSubscriptionLinkedTenantCommand
            {
                Label = "Test Tenantxax",
                LinkedUsersIds = [TestData.UserId2],
                SubscriptionId = TestData.SubscriptionId3,
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/subowner/subscriptions/selected/tenants", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("TENANT_SUB_IS_NOT_IN_SELECTED_SUBSCRIPTION", result.Extensions["errors"]!.ToString());
            
        }
    }
}

