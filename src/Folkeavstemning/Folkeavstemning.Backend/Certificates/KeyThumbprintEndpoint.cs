using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace Folkeavstemning.Api.Certificates;

public static class KeyThumbprintEndpoint
{
    public static void MapKeyThumbprintsEndpoint(this IEndpointRouteBuilder builder) =>
        builder.MapGet("/keys/thumbprints", GetThumbprints)
            .AllowAnonymous();

    public static IResult GetThumbprints(IOptions<Keys> keys) =>
        Results.Ok(
            keys.Value.ToDictionary(
                x => x.Key,
                x => GetThumbprintsFor(x.Value)
            )
        );

    private static string Hash(string file)
    {
        using var fileStream = File.OpenRead(file);
        return Convert.ToHexString(SHA256.HashData(fileStream));
    }

    private static object GetThumbprintsFor(KeyLocations keyLocations) => new KeyThumbprint(Hash(keyLocations.EncryptionPublic), Hash(keyLocations.SigningPrivate), TryOpen(keyLocations.SigningPrivate, keyLocations.SigningPrivatePassword));

    private static bool TryOpen(string file, string password)
    {
        try
        {
            var rsa = RSA.Create();
            rsa.ImportFromEncryptedPem(File.ReadAllText(file), password);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    private record KeyThumbprint(string EncryptionPublic, string SigningPrivate, bool SigningPasswordCorrect);
}
