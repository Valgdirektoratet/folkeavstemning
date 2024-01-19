using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NodaTime;
using Resultat.Api;
using Resultat.Api.Certificates;
using Resultat.Api.Crypto;
using Resultat.Api.Database;
using Resultat.Api.Prometheus;
using Resultat.Api.Stemmegivninger;
using Serilog;
using Shared.Authentication;
using Shared.Configuration;
using Shared.Health;
using Shared.Logging;
using SystemClock = NodaTime.SystemClock;

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
builder.AddPrometheus();

builder.Services.AddMemoryCache();

builder.Services.AddOptions<Keys>()
    .BindConfiguration("Keys")
    .Validate((Keys keys, ILogger<Keys> logger) => keys.Validate(logger))
    .ValidateOnStart();

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddSwaggerEx();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services.AddSingleton<IClock>(_ => SystemClock.Instance);
builder.Services.AddSingleton<ValidationService>();

builder.AddBaseAuthenticationEx();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ResultatContext>("database");

var app = builder.Build();
app.UseSwaggerEx("Resultat");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();
app.MapControllers();

app.UseOpenTelemetryPrometheusScrapingEndpoint(context =>
{
    var promConfig = context.RequestServices.GetRequiredService<IOptions<PrometheusConfig>>().Value;
    return context.Request.Path == promConfig.Endpoint && context.Connection.LocalPort == promConfig.Port;
});

app.MapStemmegivningerEndpoint();
app.MapKeyThumbprintsEndpoint();

app.MapHealthChecks("/_health", new HealthCheckOptions{
    ResponseWriter = CustomResponseWriter.WriteResponse
}).AllowAnonymous();

app.Run();
