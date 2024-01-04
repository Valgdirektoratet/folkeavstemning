using Folkeavstemning.Api.Certificates;
using Folkeavstemning.Api.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace Folkeavstemning.Api.Prometheus;

public class ManntallsKryssCountGage : IHostedService
{
    private readonly IDbContextFactory<FolkeavstemningContext> _dbContextFactory;
    private readonly IMemoryCache _memoryCache;

    public ManntallsKryssCountGage(IDbContextFactory<FolkeavstemningContext> dbContextFactory, IMemoryCache memoryCache)
    {
        _dbContextFactory = dbContextFactory;
        _memoryCache = memoryCache;
    }

    private int GetCount(string key) => _memoryCache.GetOrCreate($"{Meters.ManntallsKryssCountMeter.Name}_{key}", entry =>
    {
        using var ctx = _dbContextFactory.CreateDbContext();
        var i = ctx.Stemmegivninger.Count(x => x.FolkeavstemningId == key);

        entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

        return i;
    });

    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var key in FolkeavstemningsKonfigurasjon.Folkeavstemninger.Select(x => x.FolkeavstemningId))
        {
            Meters.ManntallsKryssCountMeter.CreateObservableGauge(
                name: $"kryss-i-manntall-{key.ToLinuxPortableCharacterSet()}",
                () => GetCount(key),
                unit: "kryss",
                description: $"Kryss i manntall for {key}");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
