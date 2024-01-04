using Resultat.Api.Stemmemottak;

namespace Resultat.Api;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication application)
    {
        application.MapPost("stemmemottak/{folkeavstemningId}", StemmemottakEndpoint.RegisterKryptertStemme)
            .Produces<string>()
            .WithGroupName("Stemmemottak")
            .RequireAuthorization();

        // application.MapPost("resultat/{folkeavstemningId}", ResultatEndpoint.DekrypterStemmer)
        //     .WithTags("2. Dekrypter stemmer og regn ut resultat")
        //     .RequireAuthorization();
    }
}

