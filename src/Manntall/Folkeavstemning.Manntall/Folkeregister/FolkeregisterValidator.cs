using Manntall.Backend.Database;
using Manntall.Backend.Folkeregister.Integration;
using NodaTime;
using Bostedsadresse = Manntall.Backend.Database.Bostedsadresse;
using VegadresseForPost = Manntall.Backend.Database.VegadresseForPost;

namespace Manntall.Backend.Folkeregister;

public static class FolkeregisterValidator
{
    public static PersonEntity Validate(string folkeavstemningId, Oppslag oppslag)
    {
        ArgumentNullException.ThrowIfNull(oppslag.Folkeregisterperson);
        var person = oppslag.Folkeregisterperson;

        var (fornavn, etternavn, mellomnavn) = GetName(person);
        var kjønn = GetKjønn(person);
        var fødselsdato = GetFødselsdato(person);
        var folkeregisterStatus = GetFolkeregisterStatus(person);
        var identifikasjonsnummer = GetIdentifikasjonsnummer(person);
        var bostedsadresse = GetBostedsadresse(person);
        var postboksAdresse = GetPostboksadresse(person);
        var vegadresseForPost = GetVegadresseForPost(person);
        var oppholdsadresse = GetOppholdsadresse(person);

        return new PersonEntity
        {
            FolkeavstemningId = folkeavstemningId,
            Fornavn = fornavn,
            Etternavn = etternavn,
            Mellomnavn = mellomnavn,
            Kjønn = kjønn,
            Fødselsdato = LocalDate.FromDateTime(fødselsdato),
            FolkeregisterStatus = folkeregisterStatus,
            Identifikasjonsnummer = identifikasjonsnummer,
            Bostedsadresse = bostedsadresse,
            PostboksAdresse = postboksAdresse,
            VegadresseForPost = vegadresseForPost,
            Oppholdsadresse = oppholdsadresse,
        };
    }

    private static (string Fornavn, string Etternavn, string? Mellomnavn) GetName(Folkeregisterperson oppslag)
    {
        var navn = oppslag.Navn?.FirstOrDefault(x => x.ErGjeldende == true);
        if (navn != null)
        {
            return (navn.Fornavn, navn.Etternavn, navn.Mellomnavn);
        }

        throw new FolkeregisterException("Missing Navn");
    }

    private static Kjønn GetKjønn(Folkeregisterperson person)
    {
        var kjoenn = person.Kjoenn?.FirstOrDefault(x => x.ErGjeldende == true);
        return kjoenn switch
        {
            { Kjoenn: PersonkjoennKjoenn.Kvinne } => Kjønn.Kvinne,
            { Kjoenn: PersonkjoennKjoenn.Mann } => Kjønn.Mann,
            _ => Kjønn.Ukjent
        };
    }

    private static DateTime GetFødselsdato(Folkeregisterperson person)
    {
        var foedsel = person.Foedsel?.FirstOrDefault(x => x.ErGjeldende == true);
        if (foedsel?.Foedselsdato.HasValue == true)
        {
            return foedsel.Foedselsdato.Value;
        }

        throw new FolkeregisterException("Missing Fødselsdato");
    }

    private static Folkeregisterstatus GetFolkeregisterStatus(Folkeregisterperson person)
    {
        var status = person.Status?.FirstOrDefault(x => x.ErGjeldende == true);
        return status?.Status switch
        {
            FolkeregisterpersonstatusStatus.Aktiv => Folkeregisterstatus.Aktuell,
            FolkeregisterpersonstatusStatus.Bosatt => Folkeregisterstatus.Aktuell,
            FolkeregisterpersonstatusStatus.Midlertidig => Folkeregisterstatus.Aktuell,
            FolkeregisterpersonstatusStatus.Foedselsregistrert => Folkeregisterstatus.Aktuell,
            null => throw new FolkeregisterException("Missing Folkeregister status"),
            _ => Folkeregisterstatus.IkkeAktuell
        };
    }

    private static string GetIdentifikasjonsnummer(Folkeregisterperson person)
    {
        var identifikator = person.Identifikasjonsnummer?.SingleOrDefault(x =>
            x is { ErGjeldende: true, Status: FolkeregisteridentifikatorStatus.IBruk });
        return identifikator switch
        {
            { Identifikatortype: FolkeregisteridentifikatorIdentifikatortype.DNummer, FoedselsEllerDNummer: var value } => value,
            { Identifikatortype: FolkeregisteridentifikatorIdentifikatortype.Foedselsnummer, FoedselsEllerDNummer: var value } => value,
            _ => throw new FolkeregisterException("Missing Identifikasjonsnummer")
        };
    }

    private static Bostedsadresse GetBostedsadresse(Folkeregisterperson person)
    {
        var bostedsadresse = person.Bostedsadresse?.FirstOrDefault(x => x.ErGjeldende == true);
        if (bostedsadresse == null)
        {
            throw new FolkeregisterException("Missing Bostedsadresse");
        }

        return new Bostedsadresse
        {
            Adressegradering = bostedsadresse.Adressegradering.ToString(),
            Adressenavn = bostedsadresse.Vegadresse?.Adressenavn,
            Kommunenummer = bostedsadresse.Vegadresse?.Kommunenummer,
            Postnummer = bostedsadresse.Vegadresse?.Poststed?.Postnummer,
            Poststed = bostedsadresse.Vegadresse?.Poststed?.Poststedsnavn,
            Adressenummer = $"{bostedsadresse.Vegadresse?.Adressenummer?.Husnummer}{bostedsadresse.Vegadresse?.Adressenummer?.Husbokstav}",
            Bruksenhetsnummer = bostedsadresse.Vegadresse?.Bruksenhetsnummer,
            Adressetillegsnavn = bostedsadresse.Vegadresse?.Adressetilleggsnavn,
            Bruksenhetstype = bostedsadresse.Vegadresse?.Bruksenhetstype.ToString(),
            Flyttedato = bostedsadresse.Flyttedato.HasValue ? LocalDate.FromDateTime(bostedsadresse.Flyttedato.Value) : null,
            Grunnkrets = bostedsadresse.Grunnkrets.ToString(),
            Kirkekrets = bostedsadresse.Kirkekrets.ToString(),
            Skolekrets = bostedsadresse.Skolekrets.ToString(),
            Stemmekrets = bostedsadresse.Stemmekrets.ToString()
        };
    }

    private static PostboksAdresse? GetPostboksadresse(Folkeregisterperson person)
    {
        var postadresse = person.Postadresse?.FirstOrDefault(x => x.ErGjeldende == true);
        if (postadresse?.Postboksadresse == null)
        {
            return null;
        }

        return new PostboksAdresse
        {
            Adressegradering = postadresse.Adressegradering.ToString(),
            Postboks = postadresse.Postboksadresse.Postboks,
            Postnummer = postadresse.Postboksadresse.Poststed?.Postnummer,
            Poststed = postadresse.Postboksadresse.Poststed?.Poststedsnavn,
            Eier = postadresse.Postboksadresse.Postbokseier
        };
    }

    private static OppholdsAdresse? GetOppholdsadresse(Folkeregisterperson person)
    {
        var adresse = person.Oppholdsadresse?.FirstOrDefault(x => x.ErGjeldende == true);
        if (adresse == null)
        {
            return null;
        }

        return new OppholdsAdresse()
        {
            Adressegradering = adresse.Adressegradering.ToString(),
            Adressenavn = adresse.Vegadresse?.Adressenavn,
            Kommunenummer = adresse.Vegadresse?.Kommunenummer,
            Postnummer = adresse.Vegadresse?.Poststed?.Postnummer,
            Poststed = adresse.Vegadresse?.Poststed?.Poststedsnavn,
            Adressenummer = $"{adresse.Vegadresse?.Adressenummer?.Husnummer}{adresse.Vegadresse?.Adressenummer?.Husbokstav}",
            Bruksenhetsnummer = adresse.Vegadresse?.Bruksenhetsnummer,
            Adressetillegsnavn = adresse.Vegadresse?.Adressetilleggsnavn,
            Bruksenhetstype = adresse.Vegadresse?.Bruksenhetstype.ToString()
        };
    }

    private static VegadresseForPost? GetVegadresseForPost(Folkeregisterperson person)
    {
        var postadresse = person.Postadresse?.FirstOrDefault(x => x.ErGjeldende == true);
        if (postadresse?.Vegadresse == null)
        {
            return null;
        }

        return new VegadresseForPost
        {
            Adressegradering = postadresse.Adressegradering.ToString(),
            Adressenavn = postadresse.Vegadresse.Adressenavn,
            Postnummer = postadresse.Vegadresse.Poststed?.Postnummer,
            Poststed = postadresse.Vegadresse.Poststed?.Poststedsnavn,
            Adressenummer = $"{postadresse.Vegadresse?.Adressenummer?.Husnummer}{postadresse.Vegadresse?.Adressenummer?.Husbokstav}",
            CoAdressenavn = postadresse.Vegadresse?.CoAdressenavn
        };
    }
}
