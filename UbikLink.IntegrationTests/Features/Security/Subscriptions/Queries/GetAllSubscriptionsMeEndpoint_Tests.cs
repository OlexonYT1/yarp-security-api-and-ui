using System.Net.Http.Headers;
using System.Net.Http.Json;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Contracts.Subscriptions.Results;

namespace UbikLink.IntegrationTests.Features.Security.Subscriptions.Queries
{
    public class GetAllSubscriptionsMeEndpoint_Tests(AspireFixture fixture) : BaseIntegrationTest(fixture)
    {
        [Fact]
        public async Task GetAllSubscribtionsMeEndpoint_WithAdmin_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.MegAdminToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync("/security/api/v1/me/subscriptions");
            var result = await response.Content.ReadFromJsonAsync<List<SubscriptionStandardResult>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<List<SubscriptionStandardResult>>(result);
        }

        [Fact]
        public async Task GetAllSubscribtionsMeEndpoint_WithUser_Ok()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.UserToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync("/security/api/v1/me/subscriptions");
            var result = await response.Content.ReadFromJsonAsync<List<SubscriptionStandardResult>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<List<SubscriptionStandardResult>>(result);
            Assert.Contains(result, sub => sub.Id == TestData.SubscriptionId1);
        }

        [Fact]
        public async Task GetAllSubscribtionsMeEndpoint_WithOtherTenant_OK()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AspireApp.OtherToken);

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync("/security/api/v1/me/subscriptions");
            var result = await response.Content.ReadFromJsonAsync<List<SubscriptionStandardResult>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.IsType<List<SubscriptionStandardResult>>(result);
            Assert.Contains(result, sub => sub.Id == TestData.SubscriptionId2);
        }

        [Fact]
        public async Task GetAllSubscribtionsMeEndpoint_WithNoAuth_401()
        {
            // Arrange
            AspireApp.AppHttpClient.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await AspireApp.AppHttpClient.GetAsync("/security/api/v1/me/subscriptions");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
