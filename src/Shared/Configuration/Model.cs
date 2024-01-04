using System.Globalization;
using NodaTime;

namespace Shared.Configuration;

public record Folkeavstemning
{
    public required string FolkeavstemningId { get; set; }
    public string FolkeavstemningIdNormalized => FolkeavstemningId.ToLinuxPortableCharacterSet();
    public required TranslatedString Navn { get; init; }
    public required ZonedDateTime Åpner { get; init; }
    public required ZonedDateTime Lukker { get; init; }
    public required Regler Regler { get; init; }
    public required TranslatedString InformasjonHeader { get; init; }
    public required TranslatedString InformasjonBody { get; init; }
    public required Sak Sak { get; init; }
    public required int ManntallsnummerStart { get; set; }
}

public record Regler
{
    public required int IkkeFødtEtter = DateTime.Now.AddYears(-18).Year;
    public required string Kommune { get; init; }
    public required string[] Stemmekretser { get; init; }

    public required bool AvstemningMåStengeFørResultatKanHentes { get; set; }
}

public record Sak
{
    public required TranslatedString Spørsmål { get; init; }
    public required TranslatedString Beskrivelse { get; set; }
    public required Dictionary<string, TranslatedString> Svaralternativer { get; init; }
}

public record TranslatedString (string Bokmål, string Nynorsk)
{

    public static implicit operator string(TranslatedString translated) => translated.ToString();

    public override string ToString()
    {
        if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "nn")
        {
            return Nynorsk;
        }

        return Bokmål;
    }
}
