namespace Shared.Configuration;

public class PrometheusConfig
{
    public string Endpoint { get; set; } = "/metrics";
    public int Port { get; set; } = 8459;
    public bool ReportVotingStatistics { get; set; }
}
