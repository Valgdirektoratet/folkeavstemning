using Microsoft.EntityFrameworkCore;
using Resultat.Api.Database;

namespace Resultat.Api.Stemmegivninger;

public static class StemmegivningerEndpoint
{
    public static void MapStemmegivningerEndpoint(this WebApplication app) =>
        app.MapGet("stemmegivninger", (ResultatContext context) =>
                context.Stemmer.GroupBy(x => x.FolkeavstemningId)
                    .Select(x => new { FolkeavstemningId = x.Key, Count = x.Count() })
                    .ToDictionaryAsync(x => x.FolkeavstemningId, x => x.Count)).RequireAuthorization(options => options
                .AddAuthenticationSchemes("BasicAuthentication")
                .RequireAuthenticatedUser()
                .Build())
            .WithGroupName("Analyse");
}
