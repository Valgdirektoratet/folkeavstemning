using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Manntall.Backend.Database;

public static class Extensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("manntallDb");
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UseNodaTime();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContextFactory<ManntallContext>(builder => builder.UseNpgsql(dataSource, o => o.UseNodaTime()).UseSnakeCaseNamingConvention());
        services.AddDbContext<ManntallContext>(builder => builder.UseNpgsql(dataSource, o => o.UseNodaTime()).UseSnakeCaseNamingConvention());
        services.AddHostedService<DatabaseMigrator>();
    }

    public static Task<PersonEntity[]> GetAllPersonerIFolkeavstemning(this IQueryable<PersonEntity> personer, string folkeavstemningId, CancellationToken token) =>
        personer.Where(x => x.FolkeavstemningId == folkeavstemningId).ToArrayAsync(token);

    public static Task<PersonEntity[]> GetAllPersonerIFolkeavstemningWithStemmerett(this IQueryable<PersonEntity> personer, string folkeavstemningId, CancellationToken token) =>
        personer.Where(x => x.FolkeavstemningId == folkeavstemningId && x.Manntallsnummer != null).ToArrayAsync(token);
}
