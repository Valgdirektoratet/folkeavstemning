using Flurl;
using Manntall.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Folkeavstemning.Api.Services.Health;

public class ManntallHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<ManntallOptions> _manntallOptions;

    public ManntallHealthCheck (HttpClient httpClient, IOptions<ManntallOptions> manntallOptions)
    {
        _httpClient = httpClient;
        _manntallOptions = manntallOptions;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        try
        {
            var response = await _httpClient.GetAsync(_manntallOptions.Value.Url.AppendPathSegment("_health"), cancellationToken);
            
            if(response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy();
            }
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("Could not reach manntall", exception);
        }
        
        return HealthCheckResult.Unhealthy("Manntall seems to be unhealthy");
    }
}
