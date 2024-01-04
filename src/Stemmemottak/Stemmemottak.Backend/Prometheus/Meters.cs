using System.Diagnostics.Metrics;

namespace Resultat.Api.Prometheus;

public static class Meters
{
    public static readonly Meter StemmegivningerCountMeter = new("StemmegivningerCountMeter", "1.0.0");
}
