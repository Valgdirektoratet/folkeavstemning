using System.Diagnostics;
using System.Net.Sockets;
using Manntall.Backend.Folkeregister.Integration;

namespace Manntall.Backend.Folkeregister;

public class FolkeregisterPersonService
{
    private readonly IFolkeregisterClient _folkeregisterClient;
    private readonly ILogger<FolkeregisterPersonService> _logger;
    public FolkeregisterPersonService(ILogger<FolkeregisterPersonService> logger, IFolkeregisterClient folkeregisterClient)
    {
        _logger = logger;
        _folkeregisterClient = folkeregisterClient;
    }

    public async Task<Oppslag[]> GetPersoner(string[] personIds, CancellationToken token)
    {
        _logger.LogInformation("GetPersons starting");
        var stopwatch = Stopwatch.StartNew();

        List<Oppslag[]> list = new();
        var count = 0;
        foreach (var personIdChunk in personIds.Chunk(1000))
        {
            _logger.LogInformation("Get persons {Count} av {Total}", count, personIds.Length);
            var oppslag = await GetPersonsBatch(personIdChunk, token);
            list.Add(oppslag);
            count += 1000;
        }

        _logger.LogInformation("GetPersons completed in {Duration} ms", stopwatch.ElapsedMilliseconds);
        return list.SelectMany(x=>x).ToArray();
    }

    private async Task<Oppslag[]> GetPersonsBatch(string[] personIdentificationNumbers, CancellationToken token, int count = 0)
    {
        if (personIdentificationNumbers.Length > 1000) throw new ArgumentException("Cannot fetch more than 1000 persons at once", nameof(personIdentificationNumbers));
        try
        {
            var result = await _folkeregisterClient.HentPersonerAsync(new[] { "person-basis" }, false,
                new PersonBulkoppslagRequest() { FoedselsEllerDNummer = personIdentificationNumbers }, token);

            return result.Oppslag != null ? result.Oppslag.ToArray() : Array.Empty<Oppslag>();
        }
        catch (IOException e)
        {
            if (e.InnerException is SocketException { ErrorCode: 104 } && count < 4)
            {
                await Task.Delay(5000, token);
                return await GetPersonsBatch(personIdentificationNumbers, token, count + 1);
            }
            _logger.LogError(e, "Error while calling Folkeregisteret HentPersoner");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while calling Folkeregisteret HentPersoner");
            throw;
        }
    }
}
