using Flurl;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Folkeavstemning.Bff.Services.Health;

public class FolkeavstemningBackendHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public FolkeavstemningBackendHealthCheck(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        //TODO: Support multiple proxy destinations
        var proxyUrl = _configuration.GetSection("ReverseProxy")
            .GetSection("Clusters")
            .GetSection("backend")
            .GetSection("Destinations")
            .GetSection("destination1")
            .GetValue<string>("Address") ?? throw new Exception("Could not get proxy url from configuration"); //ReverseProxy__Clusters__backend__Destinations__destination1__Address
        
        try
        {
            var response = await _httpClient.GetAsync(proxyUrl.AppendPathSegment("_health"), cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy($"Reached {proxyUrl}");
            }
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy($"Could not reach {proxyUrl}", e);    
        }
        
        return HealthCheckResult.Unhealthy($"{proxyUrl} seems to be unhealthy");
    }
}
