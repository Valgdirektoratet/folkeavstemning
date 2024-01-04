namespace Folkeavstemning.Api.Database;

public class Stemmegivning
{
    public Guid Id { get; set; }

    public required string FolkeavstemningId { get; set; }

    public required string Manntallsnummer { get; set; }
}
