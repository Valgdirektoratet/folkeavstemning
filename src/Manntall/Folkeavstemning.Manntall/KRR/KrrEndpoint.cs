using Manntall.Backend.Database;
using Manntall.Backend.KRR.Integration;
using Shared.Configuration;

namespace Manntall.Backend.KRR;

public class KrrEndpoint
{
    public static async Task<IResult> UpdateDigitalKontaktdata(string folkeavstemningId,
        ILogger<KrrEndpoint> logger,
        ManntallContext context,
        KrrClient client,
        CancellationToken token)
    {
        using var scope = logger.BeginScope("{Folkeavstemning} - {Handling}", folkeavstemningId, "Oppdater Digital Kontaktdata");
        if (!FolkeavstemningsKonfigurasjon.Exists(folkeavstemningId))
        {
            return Results.NotFound();
        }

        logger.LogInformation("Starting update digital kontaktdata");

        var personer = await context.Personer.GetAllPersonerIFolkeavstemningWithStemmerett(folkeavstemningId, token);
        logger.LogInformation("Found {Count} persons", personer.Length);

        var personLookup = personer.ToDictionary(entity => entity.Identifikasjonsnummer);

        var result = await HentDigitalKontaktinfo(personLookup.Keys.Order(), client, token);

        foreach (var userResource in result)
        {
            if (personLookup.TryGetValue(userResource.Personidentifikator, out var person))
            {
                if (ErDigitalBruker(userResource))
                {
                    person.DigitalKontakt = MapContactData(userResource);
                }
                else
                {
                    person.DigitalKontakt = null;
                }
            }
        }

        await context.SaveChangesAsync(token);

        logger.LogInformation("Updated digital kontaktdata");

        return Results.Ok();
    }

    private static async Task<List<UserResource>> HentDigitalKontaktinfo(IEnumerable<string> personIdentifikatorer, KrrClient client, CancellationToken token)
    {
        var responses = new List<UserResource>();
        foreach (var chunk in personIdentifikatorer.Chunk(1000))
        {
            var response = await client.GetPersons(chunk, token);

            ArgumentNullException.ThrowIfNull(response.Personer);

            responses.AddRange(response.Personer);
        }

        return responses;
    }

    private static bool ErDigitalBruker(UserResource user) =>
        user is
        {
            Varslingsstatus: UserResourceVarslingsstatus.KanVarsles,
            Reservasjon: UserResourceReservasjon.Nei,
            Status: UserResourceStatus.Aktiv
        };

    private static DigitalKontaktData MapContactData(UserResource user) =>
        new()
        {
            Språk = user.Språk,
            Epostadresse = user.Kontaktinformasjon.Epostadresse,
            Mobiltelefonnummer = user.Kontaktinformasjon.Mobiltelefonnummer
        };
}
