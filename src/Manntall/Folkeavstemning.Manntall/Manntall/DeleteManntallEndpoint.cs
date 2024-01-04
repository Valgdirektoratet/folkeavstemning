using Manntall.Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace Manntall.Backend.Manntall;

public static class DeleteManntallEndpoint
{
    public static async Task<IResult> DeleteManntall(string folkeavstemningId, ManntallContext context, CancellationToken token)
    {
        await context.Personer.Where(x => x.FolkeavstemningId == folkeavstemningId).ExecuteDeleteAsync(token);
        return Results.Ok();
    }
}
