using Manntall.Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace Manntall.Backend.Manntall;

public static class PersonEndpoint
{
    public static async Task<IResult> GetFolkeavstemninger(string folkeavstemningId, string identifikator, ManntallContext context, CancellationToken token)
    {
        var manntallsnummer = await context.Personer
            .Where(x => x.FolkeavstemningId == folkeavstemningId)
            .Where(x => x.Identifikasjonsnummer == identifikator)
            .Where(x => x.Manntallsnummer != null)
            .Select(x => x.Manntallsnummer)
            .FirstOrDefaultAsync(cancellationToken: token);

        return Results.Ok(manntallsnummer);
    }

    public static async Task<IResult> GetAlleFolkeavstemninger(string identifikator, ManntallContext context, CancellationToken token)
    {
        var manntallsnummer = await context.Personer
            .Where(x => x.Identifikasjonsnummer == identifikator)
            .Where(x => x.Manntallsnummer != null)
            .Select(x => new {x.FolkeavstemningId, x.Manntallsnummer})
            .ToArrayAsync(cancellationToken: token);

        return Results.Ok(manntallsnummer);
    }
}
