using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Resultat.Api.Database;

public class DesignTimeFactory : IDesignTimeDbContextFactory<ResultatContext>
{
    public ResultatContext CreateDbContext(string[] args)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder("Server=localhost;Port=54220;User id=postgres;Password=postgres;Database=resultat");
        dataSourceBuilder.UseNodaTime();
        var dataSource = dataSourceBuilder.Build();

        var options = new DbContextOptionsBuilder<ResultatContext>();
        options.UseNpgsql(dataSource, o => o.UseNodaTime()).UseSnakeCaseNamingConvention();
        return new ResultatContext(options.Options);
    }
}
