using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Resultat.Api.Database;

public static class Extensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("resultatDb");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UseNodaTime();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContextFactory<ResultatContext>(builder => builder.UseNpgsql(dataSource, o => o.UseNodaTime()).UseSnakeCaseNamingConvention());
        services.AddDbContext<ResultatContext>(builder => builder.UseNpgsql(dataSource, o => o.UseNodaTime()).UseSnakeCaseNamingConvention());
        services.AddHostedService<DatabaseMigrator>();

        services.AddHostedService<DatabaseClusteringService>();
    }
}
