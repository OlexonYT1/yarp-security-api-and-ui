using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using UbikLink.Common.Api;
using UbikLink.Common.Messaging.Extensions;
using UbikLink.Proxy.Authorizations;
using UbikLink.Proxy.Services;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Config
builder.Services.Configure<ProxyToken>(
    builder.Configuration.GetSection(ProxyToken.Position));

//Cache
#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.AddRedisDistributedCache("cache");

var audienceFromConfig = builder.Configuration.GetValue<string>("Parameters:auth0-audience");
var audience = string.IsNullOrEmpty(audienceFromConfig) ? "account" : audienceFromConfig;

//Auth (for normal use)
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(options =>
//{
//    options.Authority = $"{builder.Configuration.GetValue<string>("Parameters:auth-base-url")}/";
//    options.Audience = audience;
//    //TODO: change that with config
//    options.RequireHttpsMetadata = false;
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        //TODO: !!!! remove that in production (only used to accept token from integration tests)
//        ValidateIssuer = false,
//        NameClaimType = ClaimTypes.NameIdentifier
//    };
//});

//Only to work with integration tests (because it's runing in a weird way and not use the exposed port)
//So we go with the Aspire integrated solution
//Use above with external oauth....
//TODO: adpat for prod or other oauth provider
builder.Services.AddAuthentication()
                .AddKeycloakJwtBearer(
                    serviceName: "keycloak",
                    realm: "ubik",
                    configureOptions: options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.Audience = audience;
                    });

//Messaging
var transport = builder.Configuration.GetValue<string>("Messaging:Transport") == "rabbit"
    ? TransportType.RabbitMQ
    : TransportType.AzureBus;

var msgCon = string.Empty;
if (transport == TransportType.RabbitMQ)
    msgCon = builder.Configuration.GetConnectionString("ubiklink-rabbitmq");
else
    msgCon = builder.Configuration.GetConnectionString("messaging");

builder.Services.AddMasstransitFrontend("UbiklinkProxy",
    msgCon ?? string.Empty,
    false,
    transport,
    Assembly.GetExecutingAssembly(),
    builder.Configuration.GetValue<string>("Messaging:RabbitUser") ?? string.Empty,
    builder.Configuration.GetValue<string>("Messaging:RabbitPassword") ?? string.Empty
    );

//Security module
builder.Services.AddHttpClient<UserService>(static client => client.BaseAddress = new("http://ubiklink-security-api"));

//Authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, UserInfoOkHandler>();
builder.Services.AddScoped<IAuthorizationHandler, UserRolesAuthorizationOkHandler>();

//Available policies (can be written in an extension)
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("IsUser", policy =>
        policy.Requirements.Add(new UserInfoOnlyRequirement(RoleRequirement.User)))
    .AddPolicy("IsMegaAdmin", policy =>
        policy.Requirements.Add(new UserInfoOnlyRequirement(RoleRequirement.MegaAdmin)))
    .AddPolicy("IsSubOwner", policy =>
        policy.Requirements.Add(new UserInfoOnlyRequirement(RoleRequirement.SubscriptionOwner)))
    .AddPolicy("CanReadTenant", policy =>
        policy.Requirements.Add(new UserTenantRolesOrAuthorizationsRequirement(["tenant:read"], PermissionMode.Authorization)))
    .AddPolicy("CanReadTenantAndReadUser", policy =>
        policy.Requirements.Add(new UserTenantRolesOrAuthorizationsRequirement(["tenant:read", "user:read"], PermissionMode.Authorization)))
    .AddPolicy("CanReadTenantAndWriteUserRole", policy =>
        policy.Requirements.Add(new UserTenantRolesOrAuthorizationsRequirement(["tenant:read", "user:read", "tenant-user-role:write"], PermissionMode.Authorization)))
    .AddPolicy("CanReadTenantAndReadTenantRoles", policy =>
        policy.Requirements.Add(new UserTenantRolesOrAuthorizationsRequirement(["tenant:read", "tenant-role:read"], PermissionMode.Authorization)));


//Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver()
    .AddTransforms(builderContext =>
    {
        var serviceProvider = builderContext.Services;
        builderContext.AddRequestTransform(async transformContext =>
        {
            var userService = serviceProvider.GetRequiredService<UserService>();
            var user = await userService.GetUserInfoAsync(transformContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (transformContext.Path.ToUriComponent().Contains("admin"))
            {
                transformContext.ProxyRequest.Headers.Add("x-user-id", user?.Id.ToString());
                transformContext.ProxyRequest.Headers.Add("x-is-megaadmin", user?.IsMegaAdmin.ToString());
            }
            else
            {
                if (transformContext.Path.ToUriComponent().Contains("me") || transformContext.Path.ToUriComponent().Contains("subowner"))
                {
                    transformContext.ProxyRequest.Headers.Add("x-user-id", user?.Id.ToString());
                }
                else
                {
                    transformContext.ProxyRequest.Headers.Add("x-user-id", user?.Id.ToString());
                    transformContext.ProxyRequest.Headers.Add("x-tenant-id", user?.SelectedTenantId.ToString());
                }
            }
        });
    });

builder.Services.AddHttpForwarderWithServiceDiscovery();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.Run();

