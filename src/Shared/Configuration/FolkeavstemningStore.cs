using NodaTime;
using NodaTime.Extensions;

namespace Shared.Configuration;

internal static class FolkeavstemningStore
{
    private static readonly DateTimeZone NorwayTimezone = DateTimeZoneProviders.Tzdb["Europe/Oslo"];

    public static Folkeavstemning[] Production { get; } =
    {
        new()
        {
            FolkeavstemningId = "Søgne",
            Navn = new(
                "Folkeavstemning i tidligere Søgne kommune",
                "Folkerøysting i tidlegare Søgne kommune"
            ),
            ManntallsnummerStart = 100_000,
            Åpner =  new LocalDateTime(2024, 01, 22, 09, 00, 00).WithOffset(Offset.FromHours(1)).InZone(NorwayTimezone),
            Lukker = new LocalDateTime(2024, 02, 02, 21, 00, 00).WithOffset(Offset.FromHours(1)).InZone(NorwayTimezone),
            Regler = new()
            {
                Kommune = "4204", IkkeFødtEtter = 2008, Stemmekretser = new[] { "15" }, AvstemningMåStengeFørResultatKanHentes = true
            },
            InformasjonHeader = new(
                "Folkeavstemning for innbyggere som er registrert bosatt i tidligere Søgne kommune i spørsmålet om deling av Kristiansand kommune.",
                "Folkerøysting for innbyggjarar som er registrerte busette i tidlegare Søgne kommune i spørsmålet om deling av Kristiansand kommune."
            ),
            InformasjonBody = new(
                """Les mer om gjennomføringen av folkeavstemningen på <a class="underline hover:text-primary-500 focus:ring-2 py-1 ring-primary-500 outline-none" target="_blank" href="https://valg.no/folkeavstemning">valg.no/folkeavstemning</a>""",
                """Les meir om gjennomføringa av folkerøystinga på <a class="underline hover:text-primary-500 focus:ring-2 py-1 ring-primary-500 outline-none" target="_blank" href="https://valg.no/folkeavstemning">valg.no/folkeavstemning</a>"""
            ),
            Sak = new()
            {
                Spørsmål = new(
                    "Mener du at tidligere Søgne kommune bør skilles ut av Kristiansand kommune og bli en egen kommune?",
                    "Meiner du at tidlegare Søgne kommune bør skiljast ut frå Kristiansand kommune og bli ein eigen kommune?"),
                Beskrivelse = new(
                    "Mener du at tidligere Søgne kommune bør skilles ut av Kristiansand kommune og bli en egen kommune?",
                    "Meiner du at tidlegare Søgne kommune bør skiljast ut frå Kristiansand kommune og bli ein eigen kommune?"),
                Svaralternativer =
                    new()
                    {
                        ["JA"] = new("Ja", "Ja"),
                        ["NEI"] = new("Nei", "Nei"),
                        ["BLANK STEMME"] = new("Blank stemme", "Blank stemme")
                    }
            }
        },
        new()
        {
            FolkeavstemningId = "Songdalen",
            Navn = new(
                "Folkeavstemning i tidligere Songdalen kommune",
                "Folkerøysting i tidlegare Songdalen kommune"),
            ManntallsnummerStart = 200_000,
            Åpner =  new LocalDateTime(2024, 01, 22, 09, 00, 00).WithOffset(Offset.FromHours(1)).InZone(NorwayTimezone),
            Lukker = new LocalDateTime(2024, 02, 02, 21, 00, 00).WithOffset(Offset.FromHours(1)).InZone(NorwayTimezone),
            Regler = new()
            {
                Kommune = "4204", IkkeFødtEtter = 2008, Stemmekretser = new[] { "16", "17" }, AvstemningMåStengeFørResultatKanHentes = true
            },
            InformasjonHeader = new(
                "Folkeavstemning for innbyggere som er registrert bosatt i tidligere Songdalen kommune i spørsmålet om deling av Kristiansand kommune.",
                "Folkerøysting for innbyggjarar som er registrerte busette i tidlegare Songdalen kommune i spørsmålet om deling av Kristiansand kommune."
                ),
            InformasjonBody = new(
                """Les mer om gjennomføringen av folkeavstemningen på <a class="underline hover:text-primary-500 focus:ring-2 py-1 ring-primary-500 outline-none" target="_blank" href="https://valg.no/folkeavstemning">valg.no/folkeavstemning</a>""",
                """Les meir om gjennomføringa av folkerøystinga på <a class="underline hover:text-primary-500 focus:ring-2 py-1 ring-primary-500 outline-none" target="_blank" href="https://valg.no/folkeavstemning">valg.no/folkeavstemning</a>"""
            ),
            Sak = new()
            {
                Spørsmål = new(
                    "Mener du at tidligere Songdalen kommune bør skilles ut av Kristiansand kommune og bli en egen kommune igjen?",
                    "Meiner du at tidlegare Songdalen kommune bør skiljast ut frå Kristiansand kommune og bli ein eigen kommune?"),
                Beskrivelse = new(
                    "Mener du at tidligere Songdalen kommune bør skilles ut av Kristiansand kommune og bli en egen kommune igjen?",
                    "Meiner du at tidlegare Songdalen kommune bør skiljast ut frå Kristiansand kommune og bli ein eigen kommune?"),
                Svaralternativer =
                    new()
                    {
                        ["JA"] = new("Ja", "Ja"),
                        ["NEI"] = new("Nei", "Nei"),
                        ["BLANK STEMME"] = new("Blank stemme", "Blank stemme")
                    }
            }
        }
    };

    public static Folkeavstemning[] Dev { get; } = Production.Select(f =>
        f with
        {
            Åpner = SystemClock.Instance.InZone(NorwayTimezone).GetCurrentZonedDateTime().Minus(Duration.FromDays(10)),
            Lukker = SystemClock.Instance.InZone(NorwayTimezone).GetCurrentZonedDateTime().Plus(Duration.FromDays(10)),
            Regler = f.Regler with {AvstemningMåStengeFørResultatKanHentes = false}
        }).ToArray();

    public static Folkeavstemning[] ProductionNotOpen { get; } = Production.Select(f =>
        f with
        {
            Åpner = SystemClock.Instance.InZone(NorwayTimezone).GetCurrentZonedDateTime().Plus(Duration.FromDays(1)),
            Lukker = SystemClock.Instance.InZone(NorwayTimezone).GetCurrentZonedDateTime().Plus(Duration.FromDays(10)),
        }).ToArray();

    public static Folkeavstemning[] ProductionClosed { get; } = Production.Select(f =>
        f with
        {
            Åpner = SystemClock.Instance.InZone(NorwayTimezone).GetCurrentZonedDateTime().Minus(Duration.FromDays(10)),
            Lukker = SystemClock.Instance.InZone(NorwayTimezone).GetCurrentZonedDateTime().Minus(Duration.FromDays(1)),
        }).ToArray();

    public static Folkeavstemning[] ProductionActive { get; } = Production.Select(f =>
        f with
        {
            Åpner = SystemClock.Instance.InZone(NorwayTimezone).GetCurrentZonedDateTime().Minus(Duration.FromDays(10)),
            Lukker = SystemClock.Instance.InZone(NorwayTimezone).GetCurrentZonedDateTime().Plus(Duration.FromDays(10)),
        }).ToArray();
}
