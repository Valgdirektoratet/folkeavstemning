using System.Globalization;
using System.Text;
using Manntall.Backend.Database;
using Microsoft.EntityFrameworkCore;
using Shared.Export;

namespace Manntall.Backend.Manntall;

public class ManntallEndpoint
{
    public static async Task<IResult> GetManntall(string folkeavstemningId, ManntallContext context, ILogger<ManntallEndpoint> logger, CancellationToken token)
    {
        using var scope = logger.BeginScope("{Folkeavstemning} - {Handling}", folkeavstemningId, "Hent manntall");
        var culture = CultureInfo.CreateSpecificCulture("nb-NO");
        var personer = await context.Personer
            .Where(x => x.FolkeavstemningId == folkeavstemningId && x.Manntallsnummer != null)
            .OrderBy(x=>x.Manntallsnummer)
            .Select(x => new {
#pragma warning disable CS8602 // Dereference of a possibly null reference - EF Query
                x.FolkeavstemningId, x.Manntallsnummer, x.Identifikasjonsnummer, x.FolkeregisterStatus,
                Fødselsdato = x.Fødselsdato.ToString("d", culture), x.Kjønn,
                x.Fornavn, x.Etternavn, x.Mellomnavn,
                x.Bostedsadresse.Adressegradering,
                x.Bostedsadresse.Adressenavn, x.Bostedsadresse.Adressenummer, x.Bostedsadresse.Postnummer, x.Bostedsadresse.Poststed,
                x.Bostedsadresse.Stemmekrets, x.Bostedsadresse.Grunnkrets, x.Bostedsadresse.Skolekrets,
                x.PostboksAdresse.Postboks, Postboks_Postnummer = x.PostboksAdresse.Postnummer, Postboks_Poststed = x.PostboksAdresse.Poststed,
                Postadresse = x.VegadresseForPost.Adressenavn, Postadressenummer = x.VegadresseForPost.Adressenummer, PostadressePostnummer = x.VegadresseForPost.Postnummer, PostadressePoststed = x.VegadresseForPost.Poststed, PostadresseCoAdresse = x.VegadresseForPost.CoAdressenavn,
                OppholdsAdresse_Gradering = x.Oppholdsadresse.Adressegradering, OppholdsAdresse_Adressenavn=x.Bostedsadresse.Adressenavn, OppholdsAdresse_Adressenummer=x.Bostedsadresse.Adressenummer, OppholdsAdresse_Postnummer = x.Bostedsadresse.Postnummer, OppholdsAdresse_Poststed = x.Bostedsadresse.Poststed,
                x.DigitalKontakt.Mobiltelefonnummer, x.DigitalKontakt.Epostadresse,
#pragma warning restore CS8602 // Dereference of a possibly null reference - EF Query
            })
            .ToListAsync(token);

        var exportCsv = personer.ExportCsv();

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(exportCsv)).ToArray();
        return Results.File(bytes, "application/csv", $"Manntall - {folkeavstemningId}.csv");
    }

    public static async Task<IResult>? GetManntal_Trykk(string folkeavstemningId, ManntallContext context, ILogger<ManntallEndpoint> logger, CancellationToken token)
    {
        using var scope = logger.BeginScope("{Folkeavstemning} - {Handling}", folkeavstemningId, "Hent manntall");
        var culture = CultureInfo.CreateSpecificCulture("nb-NO");
        var personer = await context.Personer
            .Where(x => x.FolkeavstemningId == folkeavstemningId && x.Manntallsnummer != null)
            .OrderBy(x=>x.Manntallsnummer)
            .Select(x => new {
#pragma warning disable CS8602 // Dereference of a possibly null reference - EF Query
                x.FolkeavstemningId, x.Manntallsnummer, Fødselsdato = x.Fødselsdato.Year,
                x.Fornavn, x.Etternavn, x.Mellomnavn,
                x.Bostedsadresse.Adressenavn, x.Bostedsadresse.Adressenummer, x.Bostedsadresse.Postnummer, x.Bostedsadresse.Poststed,
                x.PostboksAdresse.Postboks, Postboks_Postnummer = x.PostboksAdresse.Postnummer, Postboks_Poststed = x.PostboksAdresse.Poststed,
                Postadresse = x.VegadresseForPost.Adressenavn, Postadressenummer = x.VegadresseForPost.Adressenummer, PostadressePostnummer = x.VegadresseForPost.Postnummer, PostadressePoststed = x.VegadresseForPost.Poststed, PostadresseCoAdresse = x.VegadresseForPost.CoAdressenavn,
                OppholdsAdresse_Gradering = x.Oppholdsadresse.Adressegradering, OppholdsAdresse_Adressenavn=x.Bostedsadresse.Adressenavn, OppholdsAdresse_Adressenummer=x.Bostedsadresse.Adressenummer, OppholdsAdresse_Postnummer = x.Bostedsadresse.Postnummer, OppholdsAdresse_Poststed = x.Bostedsadresse.Poststed,
#pragma warning restore CS8602 // Dereference of a possibly null reference - EF Query
            })
            .ToListAsync(token);

        var exportCsv = personer.ExportCsv();

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(exportCsv)).ToArray();
        return Results.File(bytes, "application/csv", $"Manntall - Trykk - {folkeavstemningId}.csv");
    }
}
