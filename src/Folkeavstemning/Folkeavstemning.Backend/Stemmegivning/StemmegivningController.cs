using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using Folkeavstemning.Api.Certificates;
using Folkeavstemning.Api.Database;
using Manntall.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Shared.Configuration;

namespace Folkeavstemning.Api.Stemmegivning;

[Route("api/stemmegivning")]
[ApiController]
[Authorize]
[EndpointGroupName("Folkeavstemning")]
public class StemmegivningController : ControllerBase
{
    private readonly FolkeavstemningContext _context;
    private readonly IOptions<Keys> _keys;
    private readonly IClock _clock;
    private readonly ILogger<StemmegivningController> _logger;
    private readonly IManntallClient _manntallClient;

    public StemmegivningController(ILogger<StemmegivningController> logger, FolkeavstemningContext context, IManntallClient manntallClient,
        IOptions<Keys> keys, IClock clock)
    {
        _logger = logger;
        _context = context;
        _manntallClient = manntallClient;
        _keys = keys;
        _clock = clock;
    }

    /// <summary>
    ///     Setter kryss i manntall
    /// </summary>
    /// <param name="folkeavstemningId">FolkeavstemningsId der det skal avlegges stemme</param>
    /// <param name="stemmepakke">BASE64 encoded bytes - stemmepakke</param>
    [HttpPost("{folkeavstemningId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<string>> AvleggStemme(string folkeavstemningId, [FromBody] string stemmepakke)
    {
        using var scope = _logger.IsEnabled(LogLevel.Debug) ? _logger.BeginScope("Fødselsnummer: {Fødselsnummer}", User.Identity?.Name) : null;
        using var scope2 = _logger.BeginScope($"Behandler stemmerett for {folkeavstemningId}");

        var folkeavstemning = FolkeavstemningsKonfigurasjon.Get(folkeavstemningId);
        if (folkeavstemning == null)
        {
            _logger.LogWarning("Kan ikke stemme - {Avvisningsgrunn}", "Ukjent folkeavstemning");
            return BadRequest(Stemmerett.Ukjent);
        }

        var identity = User.Identity?.Name;
        if (string.IsNullOrEmpty(identity))
        {
            _logger.LogWarning("Kan ikke stemme - {Avvisningsgrunn}", "Ikke logget inn");
            return BadRequest(Stemmerett.HarIkkeStemmerett);
        }

        _logger.LogInformation("Sjekker stemmerett");

        var manntallsnummer = await _manntallClient.FindManntallsnummerForPerson(folkeavstemningId, identity);

        if (string.IsNullOrWhiteSpace(manntallsnummer))
        {
            _logger.LogWarning("Kan ikke stemme - {Avvisningsgrunn}", "Har ikke stemmerett");
            return BadRequest(Stemmerett.HarIkkeStemmerett);
        }

        var harStemt = await _context.Stemmegivninger.AnyAsync(x => x.Manntallsnummer == manntallsnummer && x.FolkeavstemningId == folkeavstemningId);
        if (harStemt)
        {
            _logger.LogWarning("Kan ikke stemme - {Avvisningsgrunn}", "Har stemt fra før");
            return BadRequest(Stemmerett.HarKryssIManntall);
        }

        if (folkeavstemning.Åpner.ToInstant() > _clock.GetCurrentInstant())
        {
            _logger.LogWarning("Kan ikke stemme - {Avvisningsgrunn}", "Avstemning ikke startet");
            return BadRequest(Stemmerett.StemmegivningIkkeStartet);
        }

        if (_clock.GetCurrentInstant() > folkeavstemning.Lukker.ToInstant())
        {
            _logger.LogWarning("Kan ikke stemme - {Avvisningsgrunn}", "Avstemning lukket");
            return BadRequest(Stemmerett.StemmegivningLukket);
        }

        if (!_keys.Value.TryGetValue(folkeavstemningId.ToLinuxPortableCharacterSet(), out var keyLocations))
        {
            _logger.LogError("Kan ikke stemme - Mangler nøkkel-konfigurasjon for {FolkeavstemningId}", folkeavstemningId);
            return Problem("Mangler nøkkel-konfigurasjon");
        }

        var signature = await SignData(keyLocations, stemmepakke);

        await _context.Stemmegivninger.AddAsync(new Database.Stemmegivning
        {
            Manntallsnummer = manntallsnummer, FolkeavstemningId = folkeavstemningId
        });

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            _logger.LogWarning("Kan ikke stemme - {Avvisningsgrunn}", "Har stemt fra før");
            return BadRequest(Stemmerett.HarKryssIManntall);
        }

        _logger.LogInformation("Velger har fått kryss i manntall");
        return Ok(signature);
    }

    private static async Task<string> SignData(KeyLocations keyLocations, string stemmepakke)
    {
        var blindedMessage = BigInteger.Parse(stemmepakke, NumberStyles.None);
        var (d, n) = await GetKey(keyLocations);

        // Blind signing: s' = m' ^ d (mod N)
        return BigInteger.ModPow(blindedMessage, d, n).ToString();
    }

    private static async Task<(BigInteger d, BigInteger n)> GetKey(KeyLocations keyLocations)
    {
        var signingPrivateKey = await System.IO.File.ReadAllTextAsync(keyLocations.SigningPrivate);

        var rsa = RSA.Create();
        rsa.ImportFromEncryptedPem(signingPrivateKey, keyLocations.SigningPrivatePassword);
        var parameters = rsa.ExportParameters(true);

        var d = new BigInteger(parameters.D!, true, true);
        var n = new BigInteger(parameters.Modulus!, true, true);

        return (d, n);
    }
}

public enum Stemmerett
{
    Ukjent,
    HarIkkeStemmerett,
    HarStemmerett,
    AvleggerStemme, // brukt i frontend
    HarKryssIManntall,
    ManglerInnsendtStemme, // brukt i frontend
    StemmegivningIkkeStartet,
    StemmegivningLukket
}
