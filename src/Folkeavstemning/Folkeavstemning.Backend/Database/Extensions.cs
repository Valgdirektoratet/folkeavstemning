using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Folkeavstemning.Api.Database;

public static class Extensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("folkeavstemningDb");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UseNodaTime();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContextFactory<FolkeavstemningContext>(builder => builder.UseNpgsql(dataSource, o => o.UseNodaTime()).UseSnakeCaseNamingConvention());
        services.AddDbContext<FolkeavstemningContext>(builder => builder.UseNpgsql(dataSource, o => o.UseNodaTime()).UseSnakeCaseNamingConvention());
        services.AddHostedService<DatabaseMigrator>();
    }
}
