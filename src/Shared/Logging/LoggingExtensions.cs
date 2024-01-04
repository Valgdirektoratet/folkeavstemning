using System.Reflection;
using Serilog;

namespace Shared.Logging;

public static class LoggingExtensions
{
    public static LoggerConfiguration WithDefaultEnrichment(this LoggerConfiguration configuration)
    {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

        return
            configuration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProcessName()
                .Enrich.WithProcessId()
                .Enrich.WithProperty("Application", assembly.GetName().Name ?? "")
                .Enrich.WithProperty("Version", assembly.GetName().Version?.ToString() ?? "");
    }
}
