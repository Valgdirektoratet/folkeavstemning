using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace Resultat.Api.Certificates;

public static class KeyThumbprintEndpoint
{
    public static void MapKeyThumbprintsEndpoint(this IEndpointRouteBuilder builder) =>
        builder.MapGet("/keys/thumbprints", GetThumbprints)
            .WithGroupName("Keys")
            .AllowAnonymous();

    public static IResult GetThumbprints(IOptions<Keys> keys) =>
        Results.Ok(
            (object?)keys.Value.ToDictionary(
                x => x.Key,
                x => GetThumbprintsFor(x.Value)
            )
        );

    private static string Hash(string file)
    {
        using var fileStream = File.OpenRead(file);
        return Convert.ToHexString(SHA256.HashData(fileStream));
    }
    private static object GetThumbprintsFor(KeyLocations keyLocations) => new KeyThumbprint(Hash(keyLocations.EncryptionPublic), Hash(keyLocations.SigningPublic));

    private record KeyThumbprint(string EncryptionPublic, string SigningPublic);
}
