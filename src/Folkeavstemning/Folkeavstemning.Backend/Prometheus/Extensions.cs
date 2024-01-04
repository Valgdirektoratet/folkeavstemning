using OpenTelemetry.Metrics;
using Shared.Configuration;

namespace Folkeavstemning.Api.Prometheus;

public static class Extensions
{
    public static void AddPrometheus(this WebApplicationBuilder builder) =>
        builder.Services.AddOpenTelemetry()
            .WithMetrics(meterProvider =>
            {
                var config = builder.Configuration.GetSection("Prometheus").Get<PrometheusConfig>();
                if (config is not null && config.ReportVotingStatistics)
                {
                    meterProvider.AddMeter(Meters.ManntallsKryssCountMeter.Name);
                    builder.Services.AddHostedService<ManntallsKryssCountGage>();
                }

                meterProvider
                    .AddAspNetCoreInstrumentation()
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();
            });
}
