using Shared.Configuration;

namespace Manntall.Backend.Folkeregister;

public class FolkeregisterEndpoints
{
    public static async Task<IResult> Import(string folkeavstemningId, ILogger<FolkeregisterEndpoints> logger, FolkeregisterImportService service, CancellationToken token)
    {
        using var scope = logger.BeginScope("{Folkeavstemning} - {Handling}", folkeavstemningId, "Folkeregister uttrekk");
        var folkeavstemning = FolkeavstemningsKonfigurasjon.Get(folkeavstemningId);
        ArgumentNullException.ThrowIfNull(folkeavstemning);
        var result = await service.CreateUttrekk(folkeavstemning, token);
        return Results.Ok(result);
    }
}
