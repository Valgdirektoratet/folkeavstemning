using System.Diagnostics;
using Manntall.Backend.Database;
using Manntall.Backend.Folkeregister.Integration;
using Shared.Configuration;

namespace Manntall.Backend.Folkeregister;

public class FolkeregisterImportService
{
    private readonly FolkeregisterBatchService _batchService;
    private readonly ManntallContext _context;
    private readonly ILogger<FolkeregisterImportService> _logger;
    private readonly FolkeregisterPersonService _personService;
    private readonly FolkeregisterUttrekksService _uttrekksService;

    public FolkeregisterImportService(ILogger<FolkeregisterImportService> logger,
        FolkeregisterBatchService batchService,
        FolkeregisterUttrekksService uttrekksService,
        FolkeregisterPersonService personService,
        ManntallContext context)
    {
        _logger = logger;
        _batchService = batchService;
        _uttrekksService = uttrekksService;
        _personService = personService;
        _context = context;
    }

    public async Task<string> CreateUttrekk(Folkeavstemning folkeavstemning, CancellationToken token)
    {
        using var scope = _logger.BeginScope("Folkeregister Uttrekk");
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starter uttrekk av Folkeregister for {Folkeavstemning}", folkeavstemning.FolkeavstemningId);

        if (_context.Personer.Any(x => x.FolkeavstemningId == folkeavstemning.FolkeavstemningId))
        {
            _logger.LogWarning("Folkeregister er allerede hentet for {Folkeavstemning}", folkeavstemning.FolkeavstemningId);
            return $"Folkeregister er allerede hentet for {folkeavstemning.FolkeavstemningId}";
        }

        var jobbId = await _uttrekksService.CreateUttrekksJobb(folkeavstemning, token);
            _logger.LogInformation("Uttrekks-jobb opprettet for {Folkeavstemning}: {JobbId}", folkeavstemning.FolkeavstemningId, jobbId);
            using var jobScope = _logger.BeginScope("JobbId: {JobbId}", jobbId);

        var personIds = await _batchService.GetPersonerInUttrekk(jobbId, token);
            _logger.LogInformation("Uttrekks-jobb hentet {Count} personidentifikatorer for {Folkeavstemning}", personIds.Length, folkeavstemning.FolkeavstemningId);

        var personer = await _personService.GetPersoner(personIds, token);
            _logger.LogInformation("Hentet {Count} personer fra Folkeregisteret for {Folkeavstemning}", personer.Length, folkeavstemning.FolkeavstemningId);

        await SavePersons(folkeavstemning, personer, token);

        _logger.LogInformation("Fullført uttrekk av Folkeregister for {Folkeavstemning} på {Duration} ms", folkeavstemning.FolkeavstemningId, stopwatch.ElapsedMilliseconds);
        return $"Fullført uttrekk av Folkeregister på {stopwatch.ElapsedMilliseconds} ms";
    }

    private async Task SavePersons(Folkeavstemning folkeavstemning, IEnumerable<Oppslag> personer, CancellationToken token)
    {
        foreach (var oppslag in personer)
        {
            using var personScope = _logger.BeginScope(new Dictionary<string, object> { ["PersonIdentificationNumber"] = oppslag.FoedselsEllerDNummer });
            var entity = FolkeregisterValidator.Validate(folkeavstemning.FolkeavstemningId, oppslag);
            await _context.Personer.AddAsync(entity, token);
        }

        await _context.SaveChangesAsync(token);
    }
}
