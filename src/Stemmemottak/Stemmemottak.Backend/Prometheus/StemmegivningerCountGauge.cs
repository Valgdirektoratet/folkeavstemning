using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Resultat.Api.Certificates;
using Resultat.Api.Database;
using Shared.Configuration;

namespace Resultat.Api.Prometheus;

public class StemmegivningerCountGauge : IHostedService
{
    private readonly IDbContextFactory<ResultatContext> _dbContextFactory;
    private readonly IMemoryCache _memoryCache;

    public StemmegivningerCountGauge(IDbContextFactory<ResultatContext> dbContextFactory, IMemoryCache memoryCache)
    {
        _dbContextFactory = dbContextFactory;
        _memoryCache = memoryCache;
    }

    private int GetCount(string key) => _memoryCache.GetOrCreate($"{Meters.StemmegivningerCountMeter.Name}_{key}", entry =>
    {
        using var ctx = _dbContextFactory.CreateDbContext();
        var i = ctx.Stemmer.Count(x => x.FolkeavstemningId == key);

        entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

        return i;
    });

    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var key in FolkeavstemningsKonfigurasjon.Folkeavstemninger.Select(x => x.FolkeavstemningId))
        {
            Meters.StemmegivningerCountMeter.CreateObservableGauge(
                name: $"stemmer-mottatt-{key.ToLinuxPortableCharacterSet()}",
                () => GetCount(key),
                unit: "stemmer",
                description: $"Antall stemmer mottatt i {key}");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
