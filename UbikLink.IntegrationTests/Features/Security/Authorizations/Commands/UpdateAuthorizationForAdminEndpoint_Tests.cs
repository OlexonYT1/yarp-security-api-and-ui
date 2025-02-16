using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Authorizations.Results;

namespace UbikLink.IntegrationTests.Features.Security.Authorizations.Commands
{
    public class UpdateAuthorizationForAdminEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task UpdateAuthorizationForAdminEndpoint_WithAdmin_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateAuthorizationCommand
            {
                Code = "UPDATED_AUTH",
                Description = "Updated Authorization",
                Version = TestData.AuthorizationId2
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/authorizations/{TestData.AuthorizationId2}", command);

            var result = await response.Content.ReadFromJsonAsync<AuthorizationStandardResult>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<AuthorizationStandardResult>(result);
            Assert.Equal(command.Code, result.Code);
        }

        [Fact]
        public async Task UpdateAuthorizationForAdminEndpoint_WithUser_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateAuthorizationCommand
            {
                Code = "UPDATED_AUTH",
                Description = "Updated Authorization",
                Version = TestData.AuthorizationId2
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/authorizations/{TestData.AuthorizationId2}", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAuthorizationForAdminEndpoint_WithUserInactive_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            var command = new UpdateAuthorizationCommand
            {
                Code = "UPDATED_AUTH",
                Description = "Updated Authorization",
                Version = TestData.AuthorizationId2
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/authorizations/{TestData.AuthorizationId2}", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAuthorizationForAdminEndpoint_WithOtherTenant_403()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            var command = new UpdateAuthorizationCommand
            {
                Code = "UPDATED_AUTH",
                Description = "Updated Authorization",
                Version = TestData.AuthorizationId2
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/authorizations/{TestData.AuthorizationId2}", command);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAuthorizationForAdminEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            var command = new UpdateAuthorizationCommand
            {
                Code = "UPDATED_AUTH",
                Description = "Updated Authorization",
                Version = TestData.AuthorizationId2
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/authorizations/{TestData.AuthorizationId2}", command);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAuthorizationForAdminEndpoint_WithExistingCode_409ProblemDetails()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateAuthorizationCommand
            {
                Code = "tenant:read",
                Description = "Updated Authorization",
                Version = TestData.AuthorizationId2
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/authorizations/{TestData.AuthorizationId2}", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(409, result.Status);
            Assert.Contains("AUTHORIZATION_ALREADY_EXISTS", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task UpdateAuthorizationForAdminEndpoint_WithEmptyCode_400ProblemDetails()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateAuthorizationCommand
            {
                Code = "", // Invalid code (empty)
                Description = "Updated Authorization",
                Version = TestData.AuthorizationId2
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/authorizations/{TestData.AuthorizationId2}", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("Code is required.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task UpdateAuthorizationForAdminEndpoint_WithCodeTooLong_400ProblemDetails()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateAuthorizationCommand
            {
                Code = new string('A', 51), // Invalid code (too long)
                Description = "Updated Authorization",
                Version = TestData.AuthorizationId2
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/authorizations/{TestData.AuthorizationId2}", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("Code must not exceed 50 characters.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task UpdateAuthorizationForAdminEndpoint_WithDescriptionTooLong_400ProblemDetails()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateAuthorizationCommand
            {
                Code = "UPDATED_AUTH",
                Description = new string('A', 501), // Invalid description (too long)
                Version = TestData.AuthorizationId2
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/authorizations/{TestData.AuthorizationId2}", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("Description must not exceed 500 characters.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task UpdateAuthorizationForAdminEndpoint_WithNonExistentId_404ProblemDetails()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new UpdateAuthorizationCommand
            {
                Code = "UPDATED_AUTH",
                Description = "Updated Authorization",
                Version = TestData.AuthorizationId2
            };

            // Act
            var response = await AspireApp.AppHttpClient
                .PutAsJsonAsync($"/security/api/v1/admin/authorizations/{Guid.NewGuid()}", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(404, result.Status);
            Assert.Contains("AUTHORIZATION_NOT_FOUND", result.Extensions["errors"]!.ToString());
        }
    }
}
