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
using UbikLink.Security.Contracts.Users.Commands;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.IntegrationTests.Features.Security.Tenants.Commands
{
    public class UpdateTenantLinkedUserEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task UpdateTenantLinkedUserEndpoint_WithUser_Ok()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateUserWithTenantRoleIdsCommand
            {
                TenantRoleIds = [TestData.RoleId1,TestData.RoleId2],
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync($"/security/api/v1/tenants/{TestData.TenantId1}/users/{TestData.UserId1}/update-roles", command);
            var result = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTenantLinkedUserEndpoint_WithNoAuth_401()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            var command = new UpdateUserWithTenantRoleIdsCommand
            {
                TenantRoleIds = [TestData.RoleId1, TestData.RoleId2, TestData.RoleIdToDel1],
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync($"/security/api/v1/tenants/{TestData.TenantId1}/users/{TestData.UserId1}/update-roles", command);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTenantLinkedUserEndpoint_WithInvalidUser_403()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            var command = new UpdateUserWithTenantRoleIdsCommand
            {
                TenantRoleIds = [TestData.RoleId1, TestData.RoleId2, TestData.RoleIdToDel1],
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync($"/security/api/v1/tenants/{TestData.TenantId1}/users/{TestData.UserId1}/update-roles", command);

            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateTenantLinkedUserEndpoint_WithInvalidTenantRoleIds_400()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateUserWithTenantRoleIdsCommand
            {
                TenantRoleIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()], // Invalid role IDs
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync($"/security/api/v1/tenants/{TestData.TenantId1}/users/{TestData.UserId1}/update-roles", command);

            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(400, result.Status);
            Assert.Contains("ROLE_FOR_TENANTUSER_NOT_FOUND", result.Extensions["errors"]!.ToString()!);
        }

        [Fact]
        public async Task UpdateTenantLinkedUserEndpoint_WithInvalidUserId_404()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateUserWithTenantRoleIdsCommand
            {
                TenantRoleIds = [TestData.RoleId1, TestData.RoleId2, TestData.RoleIdToDel1],
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync($"/security/api/v1/tenants/{TestData.TenantId1}/users/{Guid.NewGuid()}/update-roles", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(404, result.Status);
            Assert.Contains("USER_NOT_FOUND", result.Extensions["errors"]!.ToString()!);
        }

        [Fact]
        public async Task UpdateTenantLinkedUserEndpoint_WithInvalidTenantId_404()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            var command = new UpdateUserWithTenantRoleIdsCommand
            {
                TenantRoleIds = [TestData.RoleId1, TestData.RoleId2, TestData.RoleIdToDel1],
            };

            //Act
            var response = await AspireApp.AppHttpClient
                .PostAsJsonAsync($"/security/api/v1/tenants/{Guid.NewGuid()}/users/{TestData.UserId1}/update-roles", command);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<ProblemDetails>(result);
            Assert.Equal(404, result.Status);
            Assert.Contains("TENANT_NOT_FOUND", result.Extensions["errors"]!.ToString()!);
        }
    }
}
