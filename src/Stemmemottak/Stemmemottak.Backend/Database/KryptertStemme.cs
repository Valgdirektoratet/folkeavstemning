namespace Resultat.Api.Database;

public class KryptertStemme
{
    public Guid Id { get; set; }
    public required string FolkeavstemningId { get; set; }

    public required string Data { get; set; }
    public required string Signatur { get; set; }
}
