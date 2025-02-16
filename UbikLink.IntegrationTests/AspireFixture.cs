using Aspire.Hosting;
using Aspire.Hosting.Postgres;
using LanguageExt.ClassInstances;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace UbikLink.IntegrationTests
{
    public class AspireFixture : IAsyncLifetime
    {
        public IDistributedApplicationTestingBuilder AppHost { get; private set; } = default!;
        public HttpClient AppHttpClient { get; private set; } = default!;
        public HttpClient AuthHttpClient { get; private set; } = default!;
        public ResourceNotificationService ResourceNotificationService { get; private set; } = default!;
        public DistributedApplication? App { get; private set; }
        public IConfiguration Configuration { get; private set; } = default!;

        internal string MegAdminToken { get; private set; } = default!;
        internal string UserToken { get; private set; } = default!;
        internal string OtherToken { get; private set; } = default!;
        internal string UserInactiveToken { get; private set; } = default!;

        public async Task InitializeAsync()
        {
            //var logger = new TestOutputHelper();

            AppHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.UbikLink_AppHost>();

            //AppHost.Services.AddLogging((builder) => builder.AddXUnit(logger));

            AppHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });


            //Remove useless resources for testing
            RemoveNotNeededResourcesForTesting();

            //Change config for testing
            ModifyResourcesForTesting();

            App = await AppHost.BuildAsync();

            var log = App.Services.GetRequiredService<IConfiguration>();

            ResourceNotificationService = App.Services
                .GetRequiredService<ResourceNotificationService>();

            await App.StartAsync();

            Configuration = App.Services.GetRequiredService<IConfiguration>();

            AppHttpClient = App.CreateHttpClient("ubiklink-proxy");
            AuthHttpClient = App.CreateHttpClient("keycloak");
            AuthHttpClient.BaseAddress = new Uri(AuthHttpClient.BaseAddress!, "realms/ubik/protocol/openid-connect/token");

            //If other auth than keycloak for integration tests
            //AuthHttpClient = App.Services.GetRequiredService<IHttpClientFactory>().CreateClient("auth-httpclient");
            //AuthHttpClient.BaseAddress = new Uri($"{Configuration.GetValue<string>("Parameters:auth-token-url")}");

            //TODO: change that to a better way to wait for the resources (can comment when running locally)
            //Fake delay to be sure the keycloak and rabbit are running in github action... sad
            await Task.Delay(120000);

            //Tokens
            MegAdminToken = await GetAccessTokenAsync(TokenType.MegaAdmin);
            UserToken = await GetAccessTokenAsync(TokenType.User);
            OtherToken = await GetAccessTokenAsync(TokenType.OtherTenant);
            UserInactiveToken = await GetAccessTokenAsync(TokenType.Inactive);

            await ResourceNotificationService.WaitForResourceAsync(
                "ubiklink-proxy",
                KnownResourceStates.Running
                )
                .WaitAsync(TimeSpan.FromSeconds(30));
        }

        private void RemoveNotNeededResourcesForTesting()
        {
            var pgAdminResources = AppHost.Resources
                .Where(r => r.GetType() == typeof(PgAdminContainerResource))
                .ToList();

            foreach (var pgAdmin in pgAdminResources)
            {
                AppHost.Resources.Remove(pgAdmin);
            }
        }

        private void ModifyResourcesForTesting()
        {
            var cache = AppHost.Resources.Where(r => r.Name == "cache")
                .FirstOrDefault();

            var db = AppHost.Resources.Where(r => r.Name == "ubiklink-postgres")
                .FirstOrDefault();

            var rabbit = AppHost.Resources.Where(r => r.Name == "ubiklink-rabbitmq")
                .FirstOrDefault();

            var keycloak = AppHost.Resources.Where(r => r.Name == "keycloak")
                .FirstOrDefault();


            if (cache != null)
            {
                var containerLifetimeAnnotation = cache.Annotations
                .OfType<ContainerLifetimeAnnotation>()
                .FirstOrDefault();

                if (containerLifetimeAnnotation != null)
                {
                    cache.Annotations.Remove(containerLifetimeAnnotation);
                }
            }

            if (rabbit != null)
            {
                var containerLifetimeAnnotation = rabbit.Annotations
                .OfType<ContainerLifetimeAnnotation>()
                .FirstOrDefault();

                if (containerLifetimeAnnotation != null)
                {
                    rabbit.Annotations.Remove(containerLifetimeAnnotation);
                }
            }

            if (keycloak != null)
            {
                //var port = keycloak.Annotations.OfType<EndpointAnnotation>().FirstOrDefault();
                //if (port != null && port.Port == 8080)
                //{
                //    port.Port = 8081;
                //}

                var containerLifetimeAnnotation = keycloak.Annotations
                .OfType<ContainerLifetimeAnnotation>()
                .FirstOrDefault();

                if (containerLifetimeAnnotation != null)
                {
                    keycloak.Annotations.Remove(containerLifetimeAnnotation);
                }
            }


            if (db != null)
            {
                var containerLifetimeAnnotation = db.Annotations
                .OfType<ContainerLifetimeAnnotation>()
                .FirstOrDefault();
                if (containerLifetimeAnnotation != null)
                {
                    db.Annotations.Remove(containerLifetimeAnnotation);
                }

                var dataVolumeAnnotation = db.Annotations
                .OfType<ContainerMountAnnotation>()
                .FirstOrDefault();
                if (dataVolumeAnnotation != null)
                {
                    db.Annotations.Remove(dataVolumeAnnotation);
                }
            }
        }

        private async Task<string> GetAccessTokenAsync(TokenType tokenType)
        {
            var dict = new Dictionary<string, string>();
            switch (tokenType)
            {
                case TokenType.MegaAdmin:
                    dict = ValuesForMegaAdmin();
                    break;
                case TokenType.OtherTenant:
                    dict = ValuesForUserOtherTenant();
                    break;
                case TokenType.User:
                    dict = ValuesForUser();
                    break;
                case TokenType.Inactive:
                    dict = ValuesForUserInactive();
                    break;
            }

            HttpResponseMessage response = await AuthHttpClient.PostAsync($"", new FormUrlEncodedContent(dict));

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadFromJsonAsync<GetTokenResult>();
                if (token != null)
                    return token.AccessToken;
            }

            throw new Exception("Cannot get auth access token to continue with testing.");
        }

        private Dictionary<string, string> ValuesForMegaAdmin()
        {
            var audience = string.IsNullOrEmpty(Configuration.GetValue<string>("Parameters:auth0-audience"))
                ? "account"
                : Configuration.GetValue<string>("Parameters:auth0-audience");

            return new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                { "client_id", Configuration.GetValue<string>("Parameters:test-auth0-client-id") ?? string.Empty },
                { "client_secret", Configuration.GetValue<string>("Parameters:test-auth0-client-secret") ?? string.Empty },
                { "username", Configuration.GetValue<string>("Parameters:test-auth0-admin-username") ?? string.Empty },
                { "password", Configuration.GetValue<string>("Parameters:test-auth0-admin-password") ?? string.Empty },
                { "grant_type", "password" },
                { "audience", audience!},
                { "scope", "openid" },
            };
        }

        private Dictionary<string, string> ValuesForUser()
        {
            var audience = string.IsNullOrEmpty(Configuration.GetValue<string>("Parameters:auth0-audience"))
               ? "account"
               : Configuration.GetValue<string>("Parameters:auth0-audience");

            return new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                { "client_id", Configuration.GetValue<string>("Parameters:test-auth0-client-id") ?? string.Empty },
                { "client_secret", Configuration.GetValue<string>("Parameters:test-auth0-client-secret") ?? string.Empty },
                { "username", Configuration.GetValue<string>("Parameters:test-auth0-user1-username") ?? string.Empty },
                { "password", Configuration.GetValue<string>("Parameters:test-auth0-user1-password") ?? string.Empty },
                { "grant_type", "password" },
                { "audience", audience! },
                { "scope", "openid" },
            };
        }

        private Dictionary<string, string> ValuesForUserOtherTenant()
        {
            var audience = string.IsNullOrEmpty(Configuration.GetValue<string>("Parameters:auth0-audience"))
               ? "account"
               : Configuration.GetValue<string>("Parameters:auth0-audience");

            return new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                { "client_id", Configuration.GetValue<string>("Parameters:test-auth0-client-id") ?? string.Empty },
                { "client_secret", Configuration.GetValue<string>("Parameters:test-auth0-client-secret") ?? string.Empty },
                { "username", Configuration.GetValue<string>("Parameters:test-auth0-user2-username") ?? string.Empty },
                { "password", Configuration.GetValue<string>("Parameters:test-auth0-user2-password") ?? string.Empty },
                { "grant_type", "password" },
                { "audience", audience!},
                { "scope", "openid" },
            };
        }

        private Dictionary<string, string> ValuesForUserInactive()
        {
            var audience = string.IsNullOrEmpty(Configuration.GetValue<string>("Parameters:auth0-audience"))
               ? "account"
               : Configuration.GetValue<string>("Parameters:auth0-audience");

            return new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                { "client_id", Configuration.GetValue<string>("Parameters:test-auth0-client-id") ?? string.Empty },
                { "client_secret", Configuration.GetValue<string>("Parameters:test-auth0-client-secret") ?? string.Empty },
                { "username", Configuration.GetValue<string>("Parameters:test-auth0-userinactive-username") ?? string.Empty },
                { "password", Configuration.GetValue<string>("Parameters:test-auth0-userinactive-password") ?? string.Empty },
                { "grant_type", "password" },
                { "audience", audience!},
                { "scope", "openid" },
            };
        }

        private record GetTokenResult
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; init; } = default!;
        }

        public async Task DisposeAsync()
        {
            AppHttpClient?.Dispose();
            AuthHttpClient?.Dispose();
            if (App != null)
            {
                if (App is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                }
                else
                {
                    App.Dispose();
                }
            }
        }
    }
    [CollectionDefinition("AspireApp collection")]
    public class AspireAppCollection : ICollectionFixture<AspireFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
