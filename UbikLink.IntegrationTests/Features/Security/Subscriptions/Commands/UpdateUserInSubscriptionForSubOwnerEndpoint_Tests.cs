using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Contracts.Subscriptions.Commands;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.IntegrationTests.Features.Security.Subscriptions.Commands
{
    public class UpdateUserInSubscriptionForSubOwnerEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task UpdateUserInSubscriptionForSubOwnerEndpoint_WithUser_OK()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateSubscriptionLinkedUserCommand
            {
                Firstname = "UpdatedFirstName",
                Lastname = "UpdatedLastName",
                IsActivated = true,
                IsSubscriptionOwner = true,
                Version = TestData.UserId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/users/{TestData.UserId1}", command);
            var result = await response.Content.ReadFromJsonAsync<UserWithInfoSubOwnerResult>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<UserWithInfoSubOwnerResult>(result);
            Assert.Equal(command.Firstname, result.Firstname);
            Assert.Equal(command.Lastname, result.Lastname);
            Assert.Equal(command.IsActivated, result.IsActivated);
            Assert.Equal(command.IsSubscriptionOwner, result.IsSubscriptionOwner);
        }

        [Fact]
        public async Task UpdateUserInSubscriptionForSubOwnerEndpoint_BadFirstName_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateSubscriptionLinkedUserCommand
            {
                Firstname = "", // Invalid Firstname
                Lastname = "UpdatedLastName",
                IsActivated = true,
                IsSubscriptionOwner = true,
                Version = TestData.UserId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/users/{TestData.UserId1}", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("Firstname is required.", result.Extensions["validationErrors"]!.ToString());
        }


        [Fact]
        public async Task UpdateUserInSubscriptionForSubOwnerEndpoint_BadUserId_404()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var nonExistentUserId = Guid.NewGuid();

            var command = new UpdateSubscriptionLinkedUserCommand
            {
                Firstname = "UpdatedFirstName",
                Lastname = "UpdatedLastName",
                IsActivated = true,
                IsSubscriptionOwner = true,
                Version = nonExistentUserId
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/users/{nonExistentUserId}", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("USER_NOT_FOUND", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task UpdateUserInSubscriptionForSubOwnerEndpoint_UserNotInSubscription_404()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateSubscriptionLinkedUserCommand
            {
                Firstname = "UpdatedFirstName",
                Lastname = "UpdatedLastName",
                IsActivated = true,
                IsSubscriptionOwner = true,
                Version = TestData.UserId2 // User not in subscription
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/users/{TestData.UserId2}", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("USER_NOT_FOUND", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task UpdateUserInSubscriptionForSubOwnerEndpoint_AtLeastOneSubscriptionActiveOwner_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateSubscriptionLinkedUserCommand
            {
                Firstname = "UpdatedFirstName",
                Lastname = "UpdatedLastName",
                IsActivated = false, // Deactivating the only owner
                IsSubscriptionOwner = true,
                Version = TestData.UserId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/users/{TestData.UserId1}", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("AT_LEAST_ONE_ACTIVATED_SUBSCRIPTION_OWNER", result.Extensions["errors"]!.ToString());
        }


        [Fact]
        public async Task UpdateUserInSubscriptionForSubOwnerEndpoint_AtLeastOneSubscriptionOwner_400()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateSubscriptionLinkedUserCommand
            {
                Firstname = "UpdatedFirstName",
                Lastname = "UpdatedLastName",
                IsActivated = true, // Deactivating the only owner
                IsSubscriptionOwner = false,
                Version = TestData.UserId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/users/{TestData.UserId1}", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Contains("AT_LEAST_ONE_ACTIVATED_SUBSCRIPTION_OWNER", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task UpdateUserInSubscriptionForSubOwnerEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            var command = new UpdateSubscriptionLinkedUserCommand
            {
                Firstname = "UpdatedFirstName",
                Lastname = "UpdatedLastName",
                IsActivated = true,
                IsSubscriptionOwner = true,
                Version = TestData.UserId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/users/{TestData.UserId1}", command);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserInSubscriptionForSubOwnerEndpoint_WithMegaAdmin_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateSubscriptionLinkedUserCommand
            {
                Firstname = "UpdatedFirstName",
                Lastname = "UpdatedLastName",
                IsActivated = true,
                IsSubscriptionOwner = true,
                Version = TestData.UserId1
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/subowner/subscriptions/selected/users/{TestData.UserId1}", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
