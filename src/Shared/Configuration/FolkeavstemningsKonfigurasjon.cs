using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shared.Configuration;

public static class FolkeavstemningsKonfigurasjon
{
    private static string? _konfigurasjon;

    public static void SetKonfigurasjon(string? konfigurasjon) => FolkeavstemningsKonfigurasjon._konfigurasjon = konfigurasjon;

    public static Folkeavstemning[] Folkeavstemninger =>
        _konfigurasjon switch
        {
            "Production" => FolkeavstemningStore.Production,

            "QA" => FolkeavstemningStore.Dev,
            "Development" => FolkeavstemningStore.Dev,
            "Test" => FolkeavstemningStore.Dev,

            "Test_IkkeÅpen" => FolkeavstemningStore.ProductionNotOpen,
            "Test_Lukket" => FolkeavstemningStore.ProductionClosed,
            "Test_Aktiv" => FolkeavstemningStore.ProductionActive,

            _ => FolkeavstemningStore.Production
        };

    public static bool Exists(string folkeavstemningId) => Folkeavstemninger.Any(x => x.FolkeavstemningId == folkeavstemningId);
    public static Folkeavstemning? Get(string folkeavstemningId) => Folkeavstemninger.FirstOrDefault(x => x.FolkeavstemningId == folkeavstemningId);
}

public class FolkeavstemningKonfigurasjonHostingService : IHostedService
{
    private readonly IHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public FolkeavstemningKonfigurasjonHostingService(IHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_environment.IsProduction())
        {
            FolkeavstemningsKonfigurasjon.SetKonfigurasjon(Environments.Production);
        }
        else
        {
            var folkeavstemningId = _configuration.GetValue<string>("FolkeavstemningConfig") ?? _environment.EnvironmentName;
            FolkeavstemningsKonfigurasjon.SetKonfigurasjon(folkeavstemningId);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

public static class FolkeavstemningKonfigurasjonExtension
{
    public static IServiceCollection ConfigureFolkeavstemningEnvironment(this IServiceCollection services) => services.AddHostedService<FolkeavstemningKonfigurasjonHostingService>();
}
