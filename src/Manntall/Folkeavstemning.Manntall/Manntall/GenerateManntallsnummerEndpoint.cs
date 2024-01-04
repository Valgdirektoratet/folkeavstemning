using Manntall.Backend.Database;
using Microsoft.EntityFrameworkCore;
using Shared.Configuration;

namespace Manntall.Backend.Manntall;

public class GenerateManntallsnummerEndpoint
{
    public static async Task<IResult> GenerateManntallsnummer(string folkeavstemningId, ILogger<GenerateManntallsnummerEndpoint> logger, ManntallContext context, CancellationToken token)
    {
        using var scope = logger.BeginScope("{Folkeavstemning} - {Handling}", folkeavstemningId, "Generer Manntallsnummer");
        var folkeavstemning = FolkeavstemningsKonfigurasjon.Get(folkeavstemningId);
        if (folkeavstemning == null)
        {
            return Results.NotFound();
        }

        if (await ManntallsnummerErGenerert(folkeavstemningId, context))
        {
            return Results.BadRequest("Already generated");
        }

        logger.LogInformation("Genererer manntallsnummer for {Folkeavstemning}", folkeavstemningId);

        var personer = await context.Personer.GetAllPersonerIFolkeavstemning(folkeavstemningId, token);

        var manntallsnummer = folkeavstemning.ManntallsnummerStart;

        foreach (var person in personer.OrderBy(p => p.Identifikasjonsnummer))
        {
            if (StemmerettKriterier.HarStemmerett(person, folkeavstemning))
            {
                manntallsnummer += 1;
                person.Manntallsnummer = $"{manntallsnummer}";
            }
        }

        await context.SaveChangesAsync(token);

        logger.LogInformation("Manntallsnummer er generert for {Folkeavstemning} - siste manntallsnummer er {Manntallsnummer}", folkeavstemning, manntallsnummer);

        return Results.Ok();
    }

    private static Task<bool> ManntallsnummerErGenerert(string folkeavstemningId, ManntallContext context) =>
        context.Personer.AnyAsync(x => x.FolkeavstemningId == folkeavstemningId && x.Manntallsnummer != null);
}

public static class StemmerettKriterier
{
    public static bool HarStemmerett(PersonEntity person, Folkeavstemning folkeavstemning)
    {
        var rettKommune = person.Bostedsadresse?.Kommunenummer == folkeavstemning.Regler.Kommune;
        var rettAlder = person.Fødselsdato.Year <= folkeavstemning.Regler.IkkeFødtEtter;
        var rettStemmekrets =
            folkeavstemning.Regler.Stemmekretser.Any(krets => person.Bostedsadresse?.Stemmekrets == krets);
        return rettKommune && rettAlder && rettStemmekrets;
    }
}
