using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Folkeavstemning.Api.Database;

public class DesignTimeFactory : IDesignTimeDbContextFactory<FolkeavstemningContext>
{
    public FolkeavstemningContext CreateDbContext(string[] args)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder("Server=localhost;Port=54020;User id=postgres;Password=postgres;Database=folkeavstemning");
        dataSourceBuilder.UseNodaTime();
        var dataSource = dataSourceBuilder.Build();

        var options = new DbContextOptionsBuilder<FolkeavstemningContext>();
        options.UseNpgsql(dataSource, o => o.UseNodaTime()).UseSnakeCaseNamingConvention();
        return new FolkeavstemningContext(options.Options);
    }
}

