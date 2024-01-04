using Microsoft.Extensions.DependencyInjection;

namespace Manntall.Client;

public static class ManntallConfiguration
{
    public static void AddManntallClient(this IServiceCollection services)
    {
        services.AddOptions<ManntallOptions>()
            .BindConfiguration("Manntall")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddScoped<IManntallClient, ManntallClient>();
    }
}
