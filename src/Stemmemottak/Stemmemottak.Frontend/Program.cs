using Flurl;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using Serilog;
using Shared.Configuration;
using Shared.Health;
using Shared.Http;
using Shared.Logging;
using Stemmemottak.Api;
using Stemmemottak.Api.Services.Health;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WithDefaultEnrichment()
    .CreateLogger();

builder.Services.AddControllers();
builder.Host.UseSerilog();
builder.Services.ConfigureFolkeavstemningEnvironment();
builder.Services.AddHttpClient();

builder.Services.AddOptions<ResultatConfig>()
    .BindConfiguration("Resultat")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<PrometheusConfig>().BindConfiguration("Prometheus");

builder.Services.AddOpenTelemetry()
    .WithMetrics(meterProvider => meterProvider
        .AddAspNetCoreInstrumentation()
        .AddProcessInstrumentation()
        .AddRuntimeInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter());

builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddOpenApiDocument();

builder.Services.AddHealthChecks()
    .AddCheck<StemmemottakBackendHealthCheck>("backend");

var app = builder.Build();

app.UseMiddleware<CustomKestrelHeaders>();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
}

app.MapControllers();

app.MapGet("/status", async (HttpContext httpContext, HttpClient httpClient, IOptions<ResultatConfig> resultatConfig,  CancellationToken ct) =>
{
    try
    {
        var result = await httpClient.GetAsync(resultatConfig.Value.Url.AppendPathSegment("_health"), ct);
        httpContext.Response.StatusCode = result.IsSuccessStatusCode ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;
    }
    catch (Exception e)
    {
        Log.Logger.Error(e, "Exception when calling /status");
        httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
    }

    return httpContext.Response.StatusCode;
}).AllowAnonymous();

app.MapGet("/", httpContext => httpContext.Response.WriteAsync("Tommel opp")).AllowAnonymous();

app.UseOpenTelemetryPrometheusScrapingEndpoint(context =>
{
    var promConfig = context.RequestServices.GetRequiredService<IOptions<PrometheusConfig>>().Value;
    return context.Request.Path == promConfig.Endpoint && context.Connection.LocalPort == promConfig.Port;
});

app.MapHealthChecks("/_health", new HealthCheckOptions{
    ResponseWriter = CustomResponseWriter.WriteResponse
}).AllowAnonymous();

app.Run();
