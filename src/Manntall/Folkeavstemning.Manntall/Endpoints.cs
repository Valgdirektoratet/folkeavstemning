using Manntall.Backend.Folkeregister;
using Manntall.Backend.KRR;
using Manntall.Backend.Manntall;

namespace Manntall.Backend;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication application)
    {
        application.MapPost("manntall/{folkeavstemningId}/uttrekk", FolkeregisterEndpoints.Import)
            .WithTags("1. Hent folkeregister")
            .RequireAuthorization();

        application.MapPost("manntall/{folkeavstemningId}/generate-manntallsnummer", GenerateManntallsnummerEndpoint.GenerateManntallsnummer)
            .WithTags("2. Generer Manntallsnummer")
            .RequireAuthorization();

        application.MapPost("manntall/{folkeavstemningId}/krr", KrrEndpoint.UpdateDigitalKontaktdata)
            .WithTags("3. Vask mot KRR")
            .RequireAuthorization();

        application.MapGet("manntall/{folkeavstemningId}/last-ned", ManntallEndpoint.GetManntall)
            .WithTags("4. Last ned manntall")
            .RequireAuthorization();

        application.MapGet("manntall/{folkeavstemningId}/last-ned-trykk", ManntallEndpoint.GetManntal_Trykk)
            .WithTags("4. Last ned manntall - Trykk")
            .RequireAuthorization();

        application.MapGet("manntall/{folkeavstemningId}/person/{identifikator}", PersonEndpoint.GetFolkeavstemninger)
            .WithTags("5. Hent stemmerett")
            .RequireAuthorization();

        application.MapGet("person/{identifikator}", PersonEndpoint.GetAlleFolkeavstemninger)
            .WithTags("5. Hent stemmerett")
            .RequireAuthorization();

        application.MapDelete("manntall/{folkeavstemningId}/delete", DeleteManntallEndpoint.DeleteManntall)
            .WithTags("6. Slett manntall")
            .RequireAuthorization();
    }
}
