using Shared.Configuration;

namespace Folkeavstemning.Api.Folkeavstemning;

public static class FolkeavstemningMapper
{
    public static FolkeavstemningDto MapToFolkeavstemningToDto(this Shared.Configuration.Folkeavstemning folkeavstemning)
    {
        return new FolkeavstemningDto
        {
            FolkeavstemningId = folkeavstemning.FolkeavstemningId,
            Navn = folkeavstemning.Navn,
            InformasjonHeader = folkeavstemning.InformasjonHeader,
            InformasjonBody = folkeavstemning.InformasjonBody,
            Åpner = folkeavstemning.Åpner.ToDateTimeUnspecified(),
            Lukker = folkeavstemning.Lukker.ToDateTimeUnspecified(),
            Sak = folkeavstemning.Sak.ToDto()
        };
    }

    private static SakDto ToDto(this Sak sak) => new() { Spørsmål = sak.Spørsmål, Svaralternativer = sak.Svaralternativer.ToDictionary(x=>x.Key, x=>x.Value.ToString()), Beskrivelse = sak.Beskrivelse };
}
