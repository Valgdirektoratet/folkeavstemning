using NodaTime;

namespace Manntall.Backend.Database;

public class PersonEntity
{
    public Guid Id { get; set; }

    public string? Manntallsnummer { get; set; }
    public required string FolkeavstemningId { get; set; }

    public required string Identifikasjonsnummer { get; set; }
    public required string Fornavn { get; set; }
    public string? Mellomnavn { get; set; }
    public required string Etternavn { get; set; }

    public required Kjønn Kjønn { get; set; }
    public required LocalDate Fødselsdato { get; set; }

    public required Folkeregisterstatus FolkeregisterStatus { get; set; }

    public Bostedsadresse? Bostedsadresse { get; set; }
    public PostboksAdresse? PostboksAdresse { get; set; }
    public VegadresseForPost? VegadresseForPost { get; set; }
    public DigitalKontaktData? DigitalKontakt { get; set; }
    public OppholdsAdresse? Oppholdsadresse { get; set; }
}

public class DigitalKontaktData
{
    public required string Språk { get; set; }
    public required string Mobiltelefonnummer { get; set; }
    public required string Epostadresse { get; set; }
}

public enum Kjønn
{
    Ukjent,
    Mann,
    Kvinne,
}

public enum Folkeregisterstatus
{
    Aktuell,
    IkkeAktuell,
}

public class Identifikasjonsnummer
{
    public required string Value { get; set; }
    public required IdentifikasjonsnummerType Type { get; set; }
}

public enum IdentifikasjonsnummerType
{
    DNummer,
    Fødselsnummer,
}

public class Bostedsadresse
{
    public required string? Adressenavn { get; set; }
    public string? Adressenummer { get; set; }
    public string? Adressetillegsnavn { get; set; }

    public required string? Postnummer { get; set; }
    public required string? Poststed { get; set; }

    public required string? Kommunenummer { get; set; }

    public string? Bruksenhetsnummer { get; set; }
    public string? Bruksenhetstype { get; set; }

    public LocalDate? Flyttedato { get; set; }
    public required string Adressegradering { get; set; }
    public string? Grunnkrets {get;set;}
    public string? Kirkekrets {get;set;}
    public string? Skolekrets {get;set;}
    public string? Stemmekrets {get;set;}
}

public class PostboksAdresse
{
    public required string Adressegradering { get; set; }
    public required string Postboks { get; set; }
    public string? Eier { get; set; }
    public required string? Postnummer { get; set; }
    public required string? Poststed { get; set; }
}

public class OppholdsAdresse
{
    public required string Adressegradering { get; set; }
    public required string? Adressenavn { get; set; }
    public required string? Kommunenummer { get; set; }
    public required string? Postnummer { get; set; }
    public required string? Poststed { get; set; }
    public required string Adressenummer { get; set; }
    public required string? Bruksenhetsnummer { get; set; }
    public required string? Adressetillegsnavn { get; set; }
    public required string? Bruksenhetstype { get; set; }
}

public class VegadresseForPost
{
    public required string Adressegradering { get; set; }
    public required string Adressenavn { get; set; }
    public string? Adressenummer { get; set; }
    public string? Bruksenhetsnummer { get; set; }
    public string? CoAdressenavn { get; set; }
    public required string? Postnummer { get; set; }
    public required string? Poststed { get; set; }
}
