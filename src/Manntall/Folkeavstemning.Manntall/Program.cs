using Manntall.Backend;
using Manntall.Backend.Database;
using Manntall.Backend.Folkeregister;
using Manntall.Backend.Folkeregister.Integration;
using Manntall.Backend.Helpers;
using Manntall.Backend.KRR.Integration;
using Manntall.Backend.Maskinporten;
using Manntall.Backend.Services.Health;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using Serilog;
using Shared.Authentication;
using Shared.Configuration;
using Shared.Health;
using Shared.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateOnBuild = true;
    options.ValidateScopes = true;
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WithDefaultEnrichment()
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.ConfigureFolkeavstemningEnvironment();

builder.Services.AddOptions<PrometheusConfig>().BindConfiguration("Prometheus");

builder.Services.AddOpenTelemetry()
    .WithMetrics(meterProvider => meterProvider
        .AddAspNetCoreInstrumentation()
        .AddProcessInstrumentation()
        .AddRuntimeInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter());

builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddSwaggerEx();
builder.AddBaseAuthenticationEx();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient();
builder.Services.ConfigureIntegrationOptions();

builder.Services.AddSingleton<MaskinportenClient>();
builder.Services.AddSingleton<KrrClient>();
builder.Services.AddSingleton<IFolkeregisterClient>(provider => new FolkeregisterClient(provider.GetRequiredService<HttpClient>(), provider.GetRequiredService<MaskinportenClient>(), provider.GetRequiredService<IOptions<FolkeregisterConfig>>()));
builder.Services.AddScoped<FolkeregisterBatchService>();
builder.Services.AddScoped<FolkeregisterPersonService>();
builder.Services.AddScoped<FolkeregisterUttrekksService>();
builder.Services.AddScoped<FolkeregisterImportService>();

builder.Services.AddSingleton<FolkeregisterHealthCheck>();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ManntallContext>("database")
    .AddCheck<FolkeregisterHealthCheck>("folkeregister");


var app = builder.Build();

app.UseSwaggerEx("Folkeregister Manntall");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.UseOpenTelemetryPrometheusScrapingEndpoint(context =>
{
    var promConfig = context.RequestServices.GetRequiredService<IOptions<PrometheusConfig>>().Value;
    return context.Request.Path == promConfig.Endpoint && context.Connection.LocalPort == promConfig.Port;
});

app.MapHealthChecks("/_health", new HealthCheckOptions
{
    ResponseWriter = CustomResponseWriter.WriteResponse
}).AllowAnonymous();

app.Run();

public partial class Program {}
