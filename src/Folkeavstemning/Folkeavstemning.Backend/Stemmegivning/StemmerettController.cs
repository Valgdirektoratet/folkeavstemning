using Folkeavstemning.Api.Database;
using Manntall.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Shared.Configuration;

namespace Folkeavstemning.Api.Stemmegivning;

[Route("api/stemmerett")]
[ApiController]
[Authorize]
[EndpointGroupName("Folkeavstemning")]
public class StemmerettController : ControllerBase
{
    private readonly FolkeavstemningContext _context;
    private readonly IClock _clock;
    private readonly IManntallClient _manntallClient;

    public StemmerettController(FolkeavstemningContext context, IManntallClient manntallClient, IClock clock)
    {
        _context = context;
        _manntallClient = manntallClient;
        _clock = clock;
    }

    [HttpGet("stemmemottak-url")]
    public string GetUrlToStemmemottak(IOptions<StemmemottakConfig> stemmemottakConfig) => stemmemottakConfig.Value.Url;

    /// <summary>
    ///     Sjekker hvilke folkeavstemninger brukeren har stemmerett i
    /// </summary>
    /// <returns>Stemmerett for brukeren i angitt folkeavstemning</returns>
    /// <response code="401">Brukeren er ikke autorisert</response>
    /// <response code="404">Ugyldig folkeavstemning</response>
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [HttpGet("stemmerett")]
    public async Task<ActionResult<Dictionary<string, Stemmerett>>> GetStemmerett()
    {
        var identity = User.Identity?.Name;
        if (string.IsNullOrEmpty(identity))
        {
            return Unauthorized();
        }

        var manntallSvar = await _manntallClient.FindFolkeavstemningForPerson(identity);

        Dictionary<string, Stemmerett> result = new();
        foreach (var folkeavstemning in FolkeavstemningsKonfigurasjon.Folkeavstemninger)
        {
            var personIManntall = manntallSvar.FirstOrDefault(x => x.FolkeavstemningId == folkeavstemning.FolkeavstemningId);
            var stemmerett = await GetStemmerett(folkeavstemning, personIManntall);
            result.Add(folkeavstemning.FolkeavstemningId, stemmerett);
        }

        return result;
    }

    private async Task<Stemmerett> GetStemmerett(Shared.Configuration.Folkeavstemning folkeavstemning, PersonIManntall? personIManntall)
    {
        if (personIManntall == null)
        {
            return Stemmerett.HarIkkeStemmerett;
        }

        var harStemt = await _context.Stemmegivninger.AnyAsync(x =>
            x.FolkeavstemningId == folkeavstemning.FolkeavstemningId &&
            x.Manntallsnummer == personIManntall.Manntallsnummer);
        if (harStemt)
        {
            return Stemmerett.HarKryssIManntall;
        }

        if (folkeavstemning.Åpner.ToInstant() > _clock.GetCurrentInstant())
        {
            return Stemmerett.StemmegivningIkkeStartet;
        }

        if (_clock.GetCurrentInstant() > folkeavstemning.Lukker.ToInstant())
        {
            return Stemmerett.StemmegivningLukket;
        }

        return Stemmerett.HarStemmerett;
    }
}
