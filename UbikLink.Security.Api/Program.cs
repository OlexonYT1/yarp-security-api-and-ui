using Asp.Versioning;
using Asp.Versioning.Builder;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Reflection;
using System.Reflection.Emit;
using UbikLink.Common.Api;
using UbikLink.Common.Errors;
using UbikLink.Common.Http;
using UbikLink.Common.Messaging;
using UbikLink.Common.Messaging.Extensions;
using UbikLink.Security.Api.Data;
using UbikLink.Security.Api.Data.Init;
using UbikLink.Security.Api.Features.Authorizations.Extensions;
using UbikLink.Security.Api.Features.Roles.Extensions;
using UbikLink.Security.Api.Features.Subscriptions.Extensions;
using UbikLink.Security.Api.Features.Tenants.Extensions;
using UbikLink.Security.Api.Features.Users.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

//Security db 
builder.Services.AddPooledDbContextFactory<SecurityDbContext>(
    options =>
    {
        options
            .UseNpgsql(builder.Configuration.GetConnectionString("ubiklink-security-db"))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSnakeCaseNamingConvention();
    });

builder.EnrichNpgsqlDbContext<SecurityDbContext>();

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

//Config
builder.Services.Configure<ProxyToken>(
    builder.Configuration.GetSection(ProxyToken.Position));

builder.Services.Configure<AuthRegisterAuthKey>(
    builder.Configuration.GetSection(AuthRegisterAuthKey.Position));

//MessageBroker 
var transport = builder.Configuration.GetValue<string>("Messaging:Transport") == "rabbit"
    ? TransportType.RabbitMQ
    : TransportType.AzureBus;

var msgCon = string.Empty;
if (transport == TransportType.RabbitMQ)
    msgCon = builder.Configuration.GetConnectionString("ubiklink-rabbitmq");
else
    msgCon = builder.Configuration.GetConnectionString("messaging");

builder.Services.AddMasstransitBackend<SecurityDbContext>("UbiklinkSecurityApi",
    msgCon ?? string.Empty,
    true,
    transport,
    Assembly.GetExecutingAssembly(),
    builder.Configuration.GetValue<string>("Messaging:RabbitUser") ?? string.Empty,
    builder.Configuration.GetValue<string>("Messaging:RabbitPassword") ?? string.Empty
    );

//Services - Features
builder.Services.AddTenantFeatures();
builder.Services.AddUserFeatures();
builder.Services.AddAuthorizationFeatures();
builder.Services.AddSubscriptionFeatures();
builder.Services.AddRoleFeatures();

//Services general
builder.Services.AddScoped<ICurrentUser,CurrentUser>();
builder.Services.AddScoped<SecurityDbContextScopedFactory>();
builder.Services.AddScoped(sp => sp.GetRequiredService<SecurityDbContextScopedFactory>().CreateDbContext());

builder.Services.AddOpenApi();
builder.Services.AddCustomProblemDetails();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
 .AddApiExplorer(options =>
 {
     options.GroupNameFormat = "'v'VVV";
     // Replace the placeholder with the actual version
     options.SubstituteApiVersionInUrl = true;
 });

builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UseExceptionHandler();

app.UseStatusCodePages();
app.UseExceptionHandler();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);
app.MapDefaultEndpoints();
app.MapOpenApi();

//Headers middelewares
app.UseWhen(
    httpContext => httpContext.Request.Path.Value!.Contains("/admin/"),

    subApp => subApp.UseMiddleware<MegaAdminUserInHeaderMiddleware>()
);

app.UseWhen(
    httpContext => httpContext.Request.Path.Value!.Contains("/proxy/"),

    subApp => subApp.UseMiddleware<ProxyTokenInHeaderMiddleware>()
);

app.UseWhen(
    httpContext => httpContext.Request.Path.Value!.Contains("/me/") 
    || httpContext.Request.Path.Value!.Contains("/subowner/"),

    subApp => subApp.UseMiddleware<MeUserInHeaderMiddleware>()
);

app.UseWhen(
    httpContext => !httpContext.Request.Path.StartsWithSegments("/scalar")
        && !httpContext.Request.Path.StartsWithSegments("/openapi")
        && !httpContext.Request.Path.Value!.Contains("/admin/")
        && !httpContext.Request.Path.Value!.Contains("/me/")
        && !httpContext.Request.Path.Value!.Contains("/proxy/")
        && !httpContext.Request.Path.Value!.Contains("/subowner/")
        && !httpContext.Request.Path.Value!.Contains("/users/register"), //no middleware on this entry point


    subApp => subApp.UseMiddleware<UserInHeaderMiddleware>()
);

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<SecurityDbContext>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(_ => _.Servers = []);

    //Db created
    await db.Database.EnsureDeletedAsync();
    await db.Database.EnsureCreatedAsync();
    await TestData.LoadAsync(db);
    await MandatoryData.LoadAsync(db);
}
else
{
    await MandatoryData.LoadAsync(db);
}



app.Run();