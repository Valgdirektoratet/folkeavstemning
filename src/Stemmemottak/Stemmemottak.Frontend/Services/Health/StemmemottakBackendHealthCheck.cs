using Flurl;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Stemmemottak.Api.Services.Health;

public class StemmemottakBackendHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<ResultatConfig> _resultatConfig;

    public StemmemottakBackendHealthCheck(HttpClient httpClient, IOptions<ResultatConfig> resultatConfig)
    {
        _httpClient = httpClient;
        _resultatConfig = resultatConfig;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var result = await _httpClient.GetAsync( _resultatConfig.Value.Url.AppendPathSegment("_health"), cancellationToken);

            if (result.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy();
            }
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy("Could not reach stemmemottak backend", e);
        }

        return HealthCheckResult.Unhealthy("Stemmemottak backend seems to be unhealthy");
    }
}
