using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Reflection;
using UbikLink.Common.Auth;
using UbikLink.Common.Frontend.Auth;
using UbikLink.Common.Frontend.HttpClients;
using UbikLink.Common.Http;
using UbikLink.Common.Messaging.Extensions;
using UbikLink.Common.RazorUI.Components;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.UI.Components;
using UbikLink.Security.UI.Httpclients;
using UbikLink.Security.UI.Shared.Httpclients;
using UbikLink.Security.UI.Shared.State;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add general services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
    //.AddAuthenticationStateSerialization(); //not really needed for the moment...

builder.Services.AddHttpContextAccessor();

//Cache && security
builder.AddRedisDistributedCache("cache");
#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddScoped<UserAndTokenCache>(); //TODO: see if we can use it as singleton

var authOptions = new AuthConfigOptions();
builder.Configuration.GetSection(AuthConfigOptions.Position).Bind(authOptions);
builder.Services.Configure<AuthConfigOptions>(
    builder.Configuration.GetSection(AuthConfigOptions.Position));

builder.Services.AddAuthForClientUI(authOptions);

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();

//Messaging
var transport = builder.Configuration.GetValue<string>("Messaging:Transport") == "rabbit"
    ? TransportType.RabbitMQ
    : TransportType.AzureBus;

var msgCon = string.Empty;
if (transport == TransportType.RabbitMQ)
    msgCon = builder.Configuration.GetConnectionString("ubiklink-rabbitmq");
else
    msgCon = builder.Configuration.GetConnectionString("messaging");

builder.Services.AddMasstransitFrontend("UbiklinkSecurityUi",
    msgCon ?? string.Empty,
    false,
    transport,
    Assembly.GetExecutingAssembly(),
    builder.Configuration.GetValue<string>("Messaging:RabbitUser") ?? string.Empty,
    builder.Configuration.GetValue<string>("Messaging:RabbitPassword") ?? string.Empty
    );

//httpclients
builder.Services.AddHttpClient(authOptions.AuthTokenHttpClientName, options =>
{
    options.BaseAddress = new Uri(authOptions.TokenUrl);
});

// User service with circuit
builder.Services.AddScoped<ClientUserService>();
builder.Services.TryAddEnumerable(
    ServiceDescriptor.Scoped<CircuitHandler, UserCircuitHandler>());

//Httpclients
// -- Me client
builder.Services.AddHttpClient<IHttpUserClient, HttpInternalUserClient>(
    static client => client.BaseAddress = new("https+http://ubiklink-proxy/security/api/v1/me/"));

//-- Security client
builder.Services
    .AddHttpClient<IHttpSecurityClient, HttpSecurityClientSrv>(
        static client => client.BaseAddress = new("https+http://ubiklink-proxy/security/api/v1/"));

builder.Services.AddScoped<AuthorizationsState>();
builder.Services.AddScoped<RolesState>();
builder.Services.AddScoped<ResponseManagerService>();

builder.Services.AddFluentUIComponents();

builder.Services.AddHttpForwarderWithServiceDiscovery();

var app = builder.Build();

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapStaticAssets();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ClientUserServiceMiddleware>();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(UbikLink.Security.UI.Client._Imports).Assembly);

app.MapDefaultEndpoints();

//Reverse proxy WASM
app.MapWasmProxy("https+http://ubiklink-proxy/security/api/v1/");

//Login, logout openIdc with httpcontext
app.MapGet("/account/login", async (HttpContext httpContext, string returnUrl = "/") =>
{
    await httpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
    {
        RedirectUri = returnUrl ?? "/"
    });
});

app.MapPost("/account/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
    {
        RedirectUri = "/"
    });
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

app.Run();
