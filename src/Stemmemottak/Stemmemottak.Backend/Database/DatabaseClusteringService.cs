using Microsoft.EntityFrameworkCore;

namespace Resultat.Api.Database;

public class DatabaseClusteringService : BackgroundService
{
    private readonly ILogger<DatabaseClusteringService> _logger;
    private readonly IDbContextFactory<ResultatContext> _dbContextFactory;

    public DatabaseClusteringService(ILogger<DatabaseClusteringService> logger, IDbContextFactory<ResultatContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation("Clustering database");
                await using var context = await _dbContextFactory.CreateDbContextAsync(stoppingToken);
                context.Database.SetCommandTimeout(TimeSpan.FromSeconds(10)); // don't bring down the database because clustering takes too long
                await context.Database.ExecuteSqlRawAsync("cluster krypterte_stemmer", stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to cluster database");
            }
        }
    }
}
