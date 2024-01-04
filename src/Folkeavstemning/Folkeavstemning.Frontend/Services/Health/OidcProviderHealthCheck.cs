using Flurl;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Folkeavstemning.Bff.Services.Health;

public class OidcProviderHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OidcProviderHealthCheck(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new ())
    {
        var authority = _configuration["OIDC:Authority"] ?? throw new Exception("OIDC Authority not configured");
        var url = authority.AppendPathSegments(".well-known", "openid-configuration");
        var response = await _httpClient.GetAsync(url, cancellationToken);

        return response.IsSuccessStatusCode ? HealthCheckResult.Healthy($"Reached {url}") : HealthCheckResult.Unhealthy($"Could not reach {url}");
    }
}
