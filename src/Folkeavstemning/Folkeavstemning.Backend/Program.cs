using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Folkeavstemning.Api.Certificates;
using Folkeavstemning.Api.ConfigureServices;
using Folkeavstemning.Api.Database;
using Folkeavstemning.Api.Prometheus;
using Folkeavstemning.Api.Services.Health;
using Folkeavstemning.Api.Stemmegivning;
using Manntall.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using NJsonSchema.Generation;
using NodaTime;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;
using Shared.Configuration;
using Shared.Health;
using Shared.Logging;
using OpenApiSecurityScheme = NSwag.OpenApiSecurityScheme;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WithDefaultEnrichment()
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.ConfigureFolkeavstemningEnvironment();

builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddOptions<Keys>()
    .BindConfiguration("Keys")
    .ValidateDataAnnotations()
    .Validate((Keys keys, ILogger<Keys> logger) => keys.Validate(logger))
    .ValidateOnStart();

builder.Services.AddOptions<StemmemottakConfig>()
    .BindConfiguration("Stemmemottak")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddCustomAuth(builder.Configuration);

builder.Services.AddManntallClient();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IClock>(_ => SystemClock.Instance);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var cultures = new[] { new CultureInfo("no-NB"), new CultureInfo("no"), new CultureInfo("nb"), new CultureInfo("nn"), new CultureInfo("no-NN") };
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;

    //options.ApplyCurrentCultureToResponseHeaders = true;
    //options.DefaultRequestCulture = new RequestCulture("no-NB");
    options.RequestCultureProviders = new IRequestCultureProvider[]
    {
        //new RouteDataRequestCultureProvider(),
        //new CookieRequestCultureProvider()
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

builder.Services.AddOpenApiDocument(options =>
{
    options.ApiGroupNames = new[] { "Manntall" };
    options.DocumentName = "Manntall";
    options.DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
    options.AddSecurity("basic", Enumerable.Empty<string>(),
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = OpenApiSecuritySchemeType.Http,
            Scheme = "basic",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "basic authentication"
        });

    options.OperationProcessors.Add(new OperationSecurityScopeProcessor("basic"));
});

builder.Services.AddOpenApiDocument(options =>
{
    options.ApiGroupNames = new[] { "Folkeavstemning" };
    options.DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
});

builder.Services.AddOptions<PrometheusConfig>().BindConfiguration("Prometheus");

builder.Services.AddMemoryCache();
builder.AddPrometheus();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<FolkeavstemningContext>("database")
    .AddCheck<ManntallHealthCheck>("manntall");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseOpenApi();
app.UseSwaggerUi();

app.UseRequestLocalization();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapKeyThumbprintsEndpoint();

app.UseOpenTelemetryPrometheusScrapingEndpoint(context =>
{
    var promConfig = context.RequestServices.GetRequiredService<IOptions<PrometheusConfig>>().Value;
    return context.Request.Path == promConfig.Endpoint && context.Connection.LocalPort == promConfig.Port;
});

app.MapHealthChecks("/_health", new HealthCheckOptions{
    ResponseWriter = CustomResponseWriter.WriteResponse
}).AllowAnonymous();

app.Run();
