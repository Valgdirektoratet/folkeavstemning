using Microsoft.Extensions.Options;

namespace Folkeavstemning.Bff.Driftsmeldinger;

public static class DriftsmeldingerRoutes
{
    public static void AddDriftsmeldinger(this WebApplication app) =>
        app.MapGet("api/driftsmeldinger", (IOptionsSnapshot<Driftsmeldinger> driftsmeldinger) =>
            Results.Ok(driftsmeldinger.Value));
}
