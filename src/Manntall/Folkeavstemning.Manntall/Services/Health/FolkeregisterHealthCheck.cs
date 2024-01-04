using Manntall.Backend.Folkeregister.Integration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Manntall.Backend.Services.Health;

public class FolkeregisterHealthCheck : IHealthCheck
{
    private readonly IFolkeregisterClient _folkeregisterClient;
    private FolkeregisterHealthCheckResponse _folkeregisterHealthCheckResponse = new(0, DateTimeOffset.UtcNow);

    public FolkeregisterHealthCheck(IFolkeregisterClient folkeregisterClient)
    {
        _folkeregisterClient = folkeregisterClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new ())
    {

        if ((DateTimeOffset.UtcNow - _folkeregisterHealthCheckResponse.Updated).TotalMinutes < 15 && _folkeregisterHealthCheckResponse.Sekvensnummer != 0)
        {
            return HealthCheckResult.Healthy("Fetched last sequence number", new Dictionary<string, object>{ {"updated", _folkeregisterHealthCheckResponse.Updated} });
        }

        try
        {
            var result = await _folkeregisterClient.SisteSekvensnummerIFeedAsync(cancellationToken);
            _folkeregisterHealthCheckResponse = new FolkeregisterHealthCheckResponse(result, DateTimeOffset.UtcNow);
            return HealthCheckResult.Healthy("Fetched last sequence number", new Dictionary<string, object>{ {"updated", _folkeregisterHealthCheckResponse.Updated} });
        }
        catch (Exception e)
        {
            return HealthCheckResult.Degraded("An exception was thrown", e, new Dictionary<string, object>{ {"updated", _folkeregisterHealthCheckResponse.Updated} });
        }
    }
}

public record FolkeregisterHealthCheckResponse(long Sekvensnummer, DateTimeOffset Updated);
