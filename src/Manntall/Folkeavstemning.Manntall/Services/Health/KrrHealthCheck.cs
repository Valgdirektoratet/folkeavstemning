using Manntall.Backend.KRR.Integration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Manntall.Backend.Services.Health;

public class KrrHealthCheck : IHealthCheck
{
    private readonly KrrClient _krrClient;

    public KrrHealthCheck(KrrClient krrClient)
    {
        _krrClient = krrClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new ())
    {
        var result = await _krrClient.GetPersons(new []{ "00000000000" }, cancellationToken);
        return HealthCheckResult.Unhealthy();
    }
}
