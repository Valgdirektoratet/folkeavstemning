using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Manntall.Backend.Database;

public class DesignTimeFactory : IDesignTimeDbContextFactory<ManntallContext>
{
    public ManntallContext CreateDbContext(string[] args)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder("Server=localhost;Port=54420;User id=postgres;Password=postgres;Database=manntall");
        dataSourceBuilder.UseNodaTime();
        var dataSource = dataSourceBuilder.Build();

        var options = new DbContextOptionsBuilder<ManntallContext>();
        options.UseNpgsql(dataSource, o => o.UseNodaTime()).UseSnakeCaseNamingConvention();
        return new ManntallContext(options.Options);
    }
}
