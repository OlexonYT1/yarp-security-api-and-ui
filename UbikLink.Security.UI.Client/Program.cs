using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using UbikLink.Common.RazorUI.Tweaks;
using UbikLink.Security.UI.Shared.Httpclients;
using UbikLink.Security.UI.Shared.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

////Persist auth (not needed for the moment => all call are protected server side, we don't care if someone try to modify the wasm app)
////No secret in the UI... so, will just pass the parameter to design the compos based on the user rights but will not check if it's really the correct ones.
//builder.Services.AddAuthorizationCore();
//builder.Services.AddCascadingAuthenticationState();
//builder.Services.AddAuthenticationStateDeserialization();

//UiServices
builder.Services.AddSingleton<AuthorizationsState>();
builder.Services.AddSingleton<RolesState>();

//Reverse proxy access
builder.Services
    .AddTransient<CookieHandler>()
    .AddHttpClient<IHttpSecurityClient, HttpSecurityClientBase>(
        client => client.BaseAddress = new Uri(new Uri(builder.HostEnvironment.BaseAddress), "api/"))
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddFluentUIComponents();
builder.Services.AddScoped<ResponseManagerService>();

await builder.Build().RunAsync();
