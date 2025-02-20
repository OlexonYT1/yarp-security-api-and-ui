//TODO: implement for dev and prod environnement

var builder = DistributedApplication.CreateBuilder(args);

//Secret
var postgresUsername = builder.AddParameter("postgres-username", secret: true);
var postgresPassword = builder.AddParameter("postgres-password", secret: true);
var securitytoken = builder.AddParameter("security-api-token", secret: true);
var authBaseUrl = builder.AddParameter("auth-base-url", secret: true);
var authMetadataUrl = builder.AddParameter("auth-metadata-url", secret: true);
var authTokenUrl = builder.AddParameter("auth-token-url", secret: true);
var authAudience = builder.AddParameter("auth0-audience", secret: true);
var securityClientAppId = builder.AddParameter("auth-security-client-app-id", secret: true);
var securityClientAppSecret = builder.AddParameter("auth-security-client-app-secret", secret: true);
var messagebusConnectionString = builder.AddParameter("messaging-url", secret: true);
var rabbitUser = builder.AddParameter("rabbit-username", secret: true);
var rabbitPassword = builder.AddParameter("rabbit-password", secret: true);
var transportType = builder.AddParameter("transport-type", secret: false);
var authTokenStoreKey = builder.AddParameter("auth-token-store-key", secret: true);

//Postgres (local)
var db = builder.AddPostgres("ubiklink-postgres", postgresUsername, postgresPassword)
    .WithDataVolume(isReadOnly: false)
    .WithEnvironment("POSTGRES_DB", "postgres")
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent);

//Azure Service Bus or rabbitmq (option)
var serviceBus = builder.AddConnectionString("messaging");

//RabbitMQ (local)
var rabbitmq = builder.AddRabbitMQ("ubiklink-rabbitmq", rabbitUser, rabbitPassword)
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

//oAuth/openIdc (local) with keycloack
var keycloak = builder.AddKeycloak("keycloak", 8080)
    .WithRealmImport("./Realm")
    .WithLifetime(ContainerLifetime.Persistent);

//Azure redis cache (for prod)
//var cache = builder.AddAzureRedis("cache");

//Redis cache (local)
var cache = builder.AddRedis("cache")
    .WithLifetime(ContainerLifetime.Persistent);

//Security API
var securityDB = db.AddDatabase("ubiklink-security-db", "ubiklink_security_db");
var securityApi = builder.AddProject<Projects.UbikLink_Security_Api>("ubiklink-security-api")
    .WithEnvironment("Proxy__Token", securitytoken)
    .WithEnvironment("ConnectionStrings__messaging", messagebusConnectionString)
    .WithEnvironment("Messaging__Transport", transportType)
    .WithEnvironment("Messaging__RabbitUser", rabbitUser)
    .WithEnvironment("Messaging__RabbitPassword", rabbitPassword)
    .WithReference(securityDB)
    .WaitFor(securityDB)
    .WithReference(rabbitmq)
    .WithReference(serviceBus);

//Proxy
var proxy = builder.AddProject<Projects.UbikLink_Proxy>("ubiklink-proxy")
    .WithEnvironment("Proxy__Token",securitytoken)
    .WithEnvironment("Parameters__auth-base-url", authBaseUrl)
    .WithEnvironment("Parameters__auth0-audience", authAudience)
    .WithEnvironment("ConnectionStrings__messaging", messagebusConnectionString)
    .WithEnvironment("Messaging__Transport", transportType)
    .WithEnvironment("Messaging__RabbitUser", rabbitUser)
    .WithEnvironment("Messaging__RabbitPassword", rabbitPassword)
    .WithReference(securityApi)
    .WithReference(keycloak)
    .WithReference(cache)
    .WithReference(serviceBus)
    .WithReference(rabbitmq)
    .WaitFor(cache)
    .WaitFor(securityApi)
    .WaitFor(keycloak);
    
//.WithReference(rabbitmq)
// .WaitFor(rabbitmq)

//Security UI
builder.AddProject<Projects.UbikLink_Security_UI>("ubiklink-security-ui")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(proxy)
    .WaitFor(proxy)
    .WithReference(serviceBus)
    .WithReference(rabbitmq)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithEnvironment("ConnectionStrings__messaging", messagebusConnectionString)
    .WithEnvironment("AuthConfig__MetadataAddress", authMetadataUrl)
    .WithEnvironment("AuthConfig__Authority", authBaseUrl)
    .WithEnvironment("AuthConfig__Audience", authAudience)
    .WithEnvironment("AuthConfig__TokenUrl", authTokenUrl)
    .WithEnvironment("AuthConfig__ClientId", securityClientAppId)
    .WithEnvironment("AuthConfig__ClientSecret", securityClientAppSecret)
    .WithEnvironment("AuthConfig__AuthTokenStoreKey", securityClientAppSecret)
    .WithEnvironment("Messaging__Transport", transportType)
    .WithEnvironment("Messaging__RabbitUser", rabbitUser)
    .WithEnvironment("Messaging__RabbitPassword", rabbitPassword);

await builder.Build().RunAsync();
