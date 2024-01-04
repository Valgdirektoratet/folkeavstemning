using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Resultat.Api.Certificates;
using Resultat.Api.Crypto;
using Resultat.Api.Database;
using Shared.Configuration;

namespace Resultat.Api.Stemmemottak;

public class StemmemottakEndpoint
{
    public static async Task<IResult> RegisterKryptertStemme(string folkeavstemningId, SignertStemmeDto dto, ResultatContext context,
        IOptions<Keys> keys, ILogger<StemmemottakEndpoint> logger, IClock clock, ValidationService validationService)
    {
        using var scope = logger.BeginScope("Behandling av stemme for {FolkeavstemningId}", folkeavstemningId);
        logger.LogInformation("Mottatt stemme - starter behandling");
        try
        {
            var folkeavstemning = FolkeavstemningsKonfigurasjon.Get(folkeavstemningId);
            if (folkeavstemning is null)
            {
                logger.LogWarning("Stemme forkastet - {Forkastelsesgrunn}", Forkastelsesgrunn.UkjentFolkeavstemning);
                return Results.BadRequest(StemmemottakResponse.Ukjent);
            }

            var kryptertStemme = ValiderDto(folkeavstemningId, dto);

            if(!await validationService.ValiderSignatur(kryptertStemme))
            {
                logger.LogWarning("Stemme forkastet - {Forkastelsesgrunn}", Forkastelsesgrunn.UgyldigSignatur);
                return Results.BadRequest(StemmemottakResponse.UgyldigSignatur);
            }

            var instant = clock.GetCurrentInstant();
            if (instant < folkeavstemning.Åpner.ToInstant())
            {
                logger.LogWarning("Stemme forkastet - {Forkastelsesgrunn}", Forkastelsesgrunn.AvstemningIkkeStartet);
                return Results.BadRequest(StemmemottakResponse.AvstemningIkkeStartet);
            }

            if (instant > folkeavstemning.Lukker.ToInstant())
            {
                logger.LogWarning("Stemme forkastet - {Forkastelsesgrunn}", Forkastelsesgrunn.AvstemningLukket);
                return Results.BadRequest(StemmemottakResponse.AvstemningLukket);
            }

            var stemmeEksisterer = await context.Stemmer.AnyAsync(x => x.Signatur == dto.Signatur);
            if (stemmeEksisterer)
            {
                logger.LogWarning("Stemme forkastet - {Forkastelsesgrunn}", Forkastelsesgrunn.DuplikatStemme);
                return Results.BadRequest(StemmemottakResponse.AlleredeLevertStemme);
            }

            await context.Stemmer.AddAsync(kryptertStemme);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                logger.LogWarning("Stemme forkastet - {Forkastelsesgrunn}", Forkastelsesgrunn.DuplikatStemme);
                return Results.BadRequest(StemmemottakResponse.AlleredeLevertStemme);
            }

            logger.LogInformation("Stemme registrert");
            return Results.Ok(StemmemottakResponse.Ok);
        }
        catch (ArgumentNullException)
        {
            logger.LogWarning("Stemme forkastet - {Forkastelsesgrunn}", Forkastelsesgrunn.UgyldigData);
            return Results.BadRequest(StemmemottakResponse.UgyldigData);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Stemme forkastet - {Forkastelsesgrunn}", Forkastelsesgrunn.UkjentFeil);
            return Results.BadRequest(StemmemottakResponse.Ukjent);
        }
    }

    private static KryptertStemme ValiderDto(string folkeavstemningId, SignertStemmeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto.Data);
        ArgumentNullException.ThrowIfNull(dto.Signatur);

        var signatur = BigInteger.Parse(dto.Signatur, NumberStyles.None);
        var normalizedSignatur = signatur.ToString("R");

        return new KryptertStemme { Data = dto.Data, Signatur = normalizedSignatur, FolkeavstemningId = folkeavstemningId };
    }

    public static class StemmemottakResponse
    {
        public const string Ok = "Ok";
        public const string Ukjent = "Ukjent";
        public const string AvstemningLukket = "AvstemningLukket";
        public const string AvstemningIkkeStartet = "AvstemningIkkeStartet";
        public const string AlleredeLevertStemme = "AlleredeLevertStemme";
        public const string UgyldigSignatur = "UgyldigSignatur";
        public const string UgyldigData = "UgyldigData";
    }
}

public class Forkastelsesgrunn
{
    public const string DuplikatStemme = "Duplikat stemme";
    public const string UgyldigSignatur = "Ugyldig signatur";
    public const string UgyldigData = "Ugyldig data";
    public const string UkjentFeil = "Ukjent feil";
    public const string UkjentFolkeavstemning = "Ukjent folkeavstemning";
    public static string AvstemningIkkeStartet = "Avstemning ikke startet";
    public static string AvstemningLukket = "Avstemning lukket";
}

internal class InvalidSignatureException : Exception
{
}

public class SignertStemmeDto
{
    public string? Data { get; set; }
    public string? Signatur { get; set; }
}
