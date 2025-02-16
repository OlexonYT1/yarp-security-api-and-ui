using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Contracts.Authorizations.Commands;
using UbikLink.Security.Contracts.Authorizations.Results;

namespace UbikLink.IntegrationTests.Features.Security.Authorizations.Commands
{
    public class AddAuthorizationForAdminEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task AddAuthorizationForAdminEndpoint_WithAdmin_Ok()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddAuthorizationCommand
            {
                Code = "TEST_AUTH",
                Description = "Test Authorization"
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations", command);

            var result = await response.Content.ReadFromJsonAsync<AuthorizationStandardResult>();

            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<AuthorizationStandardResult>(result);
            Assert.Equal(command.Code, result.Code);
        }

        [Fact]
        public async Task AddAuthorizationForAdminEndpoint_WithUser_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new AddAuthorizationCommand
            {
                Code = "TEST_AUTH",
                Description = "Test Authorization"
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations", command);

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddAuthorizationForAdminEndpoint_WithUserInactive_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserInactiveToken);

            var command = new AddAuthorizationCommand
            {
                Code = "TEST_AUTH",
                Description = "Test Authorization"
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations", command);

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddAuthorizationForAdminEndpoint_WithOtherTenant_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            var command = new AddAuthorizationCommand
            {
                Code = "TEST_AUTH",
                Description = "Test Authorization"
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations", command);

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddAuthorizationForAdminEndpoint_WithNoAuth_401()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            var command = new AddAuthorizationCommand
            {
                Code = "TEST_AUTH",
                Description = "Test Authorization"
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations", command);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AddAuthorizationForAdminEndpoint_WithExistingCode_409ProblemDetails()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddAuthorizationCommand
            {
                Code = "tenant:read",
                Description = "Test Authorization"
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(409, result.Status);
            Assert.Contains("AUTHORIZATION_ALREADY_EXISTS", result.Extensions["errors"]!.ToString());
        }

        [Fact]
        public async Task AddAuthorizationForAdminEndpoint_WithEmptyCode_400ProblemDetails()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddAuthorizationCommand
            {
                Code = "", // Invalid code (empty)
                Description = "Test Authorization"
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("Code is required.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task AddAuthorizationForAdminEndpoint_WithCodeTooLong_400ProblemDetails()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddAuthorizationCommand
            {
                Code = new string('A', 51), // Invalid code (too long)
                Description = "Test Authorization"
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("Code must not exceed 50 characters.", result.Extensions["validationErrors"]!.ToString());
        }

        [Fact]
        public async Task AddAuthorizationForAdminEndpoint_WithDescriptionTooLong_400ProblemDetails()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            var command = new AddAuthorizationCommand
            {
                Code = "TEST_AUTH",
                Description = new string('A', 501) // Invalid description (too long)
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync("/security/api/v1/admin/authorizations", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("Description must not exceed 500 characters.", result.Extensions["validationErrors"]!.ToString());
        }
    }
}
