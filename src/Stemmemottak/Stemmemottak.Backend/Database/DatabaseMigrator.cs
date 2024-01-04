using Microsoft.EntityFrameworkCore;

namespace Resultat.Api.Database;

public class DatabaseMigrator : IHostedService
{
    private readonly ILogger<DatabaseMigrator> _logger;
    private readonly IDbContextFactory<ResultatContext> _contextFactory;

    public DatabaseMigrator(ILogger<DatabaseMigrator> logger, IDbContextFactory<ResultatContext> contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Migrating database");
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await context.Database.MigrateAsync(cancellationToken: cancellationToken);
        _logger.LogInformation("Database migrated");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}