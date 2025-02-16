using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Contracts.Users.Results;

namespace UbikLink.IntegrationTests.Features.Security.Users.Queries
{
    public class GetUserMeEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task GetUserMeEndpoint_WithAdmin_Ok()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            //Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/me/authinfo");
            var result = await response.Content.ReadFromJsonAsync<UserMeResult>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<UserMeResult>(result);
            Assert.True(result.IsMegaAdmin);
        }

        [Fact]
        public async Task GetUserMeEndpoint_WithUser_Ok()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            //Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/me/authinfo");

            var result = await response.Content.ReadFromJsonAsync<UserMeResult>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<UserMeResult>(result);
            Assert.False(result.IsMegaAdmin);
        }

        [Fact]
        public async Task GetUserMeEndpoint_WithOtherTenant_Ok()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            //Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/me/authinfo");

            var result = await response.Content.ReadFromJsonAsync<UserMeResult>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<UserMeResult>(result);
            Assert.False(result.IsMegaAdmin);
        }

        [Fact]
        public async Task GetUserMeEndpoint_WithNoAuth_401()
        {
            //Arrange 
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            //Act
            var response = await AspireApp.AppHttpClient
                .GetAsync($"/security/api/v1/me/authinfo");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
