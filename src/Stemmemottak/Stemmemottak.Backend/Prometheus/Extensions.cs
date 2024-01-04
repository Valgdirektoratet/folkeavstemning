using OpenTelemetry.Metrics;
using Shared.Configuration;

namespace Resultat.Api.Prometheus;

public static class Extensions
{
    public static void AddPrometheus(this WebApplicationBuilder builder) =>
        builder.Services.AddOpenTelemetry()
            .WithMetrics(meterProvider =>
            {
                var config = builder.Configuration.GetSection("Prometheus").Get<PrometheusConfig>();
                if (config is not null && config.ReportVotingStatistics)
                {
                    meterProvider.AddMeter(Meters.StemmegivningerCountMeter.Name);
                    builder.Services.AddHostedService<StemmegivningerCountGauge>();
                }

                meterProvider
                    .AddAspNetCoreInstrumentation()
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();
            });
}
