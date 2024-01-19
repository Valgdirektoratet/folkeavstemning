using System.Text.Json;
using System.Text.Json.Serialization;
using Duende.Bff.Yarp;
using Folkeavstemning.Bff.ConfigureServices;
using Folkeavstemning.Bff.Driftsmeldinger;
using Folkeavstemning.Bff.Services.Health;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using Serilog;
using Shared.Configuration;
using Shared.Health;
using Shared.Http;
using Shared.Logging;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WithDefaultEnrichment()
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.ConfigureFolkeavstemningEnvironment();

builder.AddDuendeBff();
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddBffExtensions();

builder.Services.AddOptions<PrometheusConfig>()
    .BindConfiguration("Prometheus");

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddOptions<Driftsmeldinger>()
    .BindConfiguration("Driftsmeldinger");

builder.Services.AddOpenTelemetry()
    .WithMetrics(meterProvider => meterProvider
        .AddAspNetCoreInstrumentation()
        .AddProcessInstrumentation()
        .AddRuntimeInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter());

builder.Services.AddHealthChecks()
    .AddCheck<OidcProviderHealthCheck>("oidc")
    .AddCheck<FolkeavstemningBackendHealthCheck>("backend");

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseBff();
app.UseAuthorization();

app.MapBffManagementEndpoints();

app.UseMiddleware<CustomKestrelHeaders>();

if (!app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.UseRouting();
    app.MapFallbackToFile("index.html");
}

app.MapHealthChecks("_health", new HealthCheckOptions()
{
    ResponseWriter = CustomResponseWriter.WriteResponse
}).AllowAnonymous();

app.AddDriftsmeldinger();

app.UseOpenTelemetryPrometheusScrapingEndpoint(context =>
{
    var promConfig = context.RequestServices.GetRequiredService<IOptions<PrometheusConfig>>().Value;
    return context.Request.Path == promConfig.Endpoint && context.Connection.LocalPort == promConfig.Port;
});

app.MapReverseProxy();

app.Run();
