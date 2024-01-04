using Manntall.Backend.Folkeregister.Integration;
using Shared.Configuration;

namespace Manntall.Backend.Folkeregister;

public class FolkeregisterUttrekksService
{
    private readonly IFolkeregisterClient _folkeregisterClient;
    private readonly ILogger<FolkeregisterUttrekksService> _logger;

    public FolkeregisterUttrekksService(ILogger<FolkeregisterUttrekksService> logger, IFolkeregisterClient folkeregisterClient)
    {
        _logger = logger;
        _folkeregisterClient = folkeregisterClient;
    }

    public async Task<string> CreateUttrekksJobb(Folkeavstemning folkeavstemning, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating UttrekksJobb");

        var request = new TilpassetUttrekkJobbRequest
        {
            Kommunenummer = new Kommunenummer { Bostedskommunenummer = folkeavstemning.Regler.Kommune },
            FoedselsaarTilOgMed = folkeavstemning.Regler.IkkeFødtEtter.ToString(),
            Personstatustyper = new List<Personstatustyper>() { Personstatustyper.Bosatt}
        };

        var response = await _folkeregisterClient.LagJobbAsync(request, cancellationToken);

        if (response.Feilmelding != null)
        {
            _logger.LogError("Received error from folkeregisteret on LagJobb: {Feilmelding}", response.Feilmelding);
            throw new FolkeregisterException(response.Feilmelding);
        }
        if (string.IsNullOrEmpty(response.JobbId))
        {
            _logger.LogError("Received no JobbId from Folkeregisteret");
            throw new FolkeregisterException("Received no JobbId from Folkeregisteret");
        }

        return response.JobbId;
    }
}
