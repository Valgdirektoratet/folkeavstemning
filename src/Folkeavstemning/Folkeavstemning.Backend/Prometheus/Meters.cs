using System.Diagnostics.Metrics;

namespace Folkeavstemning.Api.Prometheus;

public static class Meters
{
    public static readonly Meter ManntallsKryssCountMeter = new("ManntallsKryssCountMeter", "1.0.0");
}
