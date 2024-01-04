namespace Folkeavstemning.Api.Folkeavstemning;

public class FolkeavstemningDto
{
    public required string FolkeavstemningId { get; set; }
    public required string Navn { get; set; }
    public required string InformasjonHeader { get; set; }
    public required string InformasjonBody { get; set; }
    public DateTime? Åpner { get; init; }
    public DateTime? Lukker { get; init; }
    public required SakDto Sak { get; init; }
}

public class SakDto
{
    public required string Beskrivelse { get; set; }
    public required string Spørsmål { get; set; }
    public required Dictionary<string, string> Svaralternativer { get; init; }
}
