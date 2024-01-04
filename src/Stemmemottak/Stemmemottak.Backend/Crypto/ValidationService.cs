using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Resultat.Api.Certificates;
using Resultat.Api.Database;
using Resultat.Api.Stemmemottak;
using Shared.Configuration;

namespace Resultat.Api.Crypto;

public static class RsaExtensions
{
    public static (BigInteger, BigInteger) GetPublicComponents(this RSA rsa)
    {
        var parameters = rsa.ExportParameters(false);

        var e = new BigInteger(parameters.Exponent!, true, true);
        var n = new BigInteger(parameters.Modulus!, true, true);
        return (e, n);
    }

    public static bool IsSignatureLengthValid(this BigInteger signature, RSA rsaKey)
    {
        var signatureBytes = signature.ToByteArray(isBigEndian: true);
        var expectedLength = rsaKey.KeySize / 8;
        var actualLength = signatureBytes.Length;

        return expectedLength + 1 >= actualLength;
    }
}
public class ValidationService
{
    private readonly ILogger<ValidationService> _logger;
    private readonly Keys _keys;
    private readonly Dictionary<string, RSA> _rsaKeys = new();

    public ValidationService(ILogger<ValidationService> logger, IOptions<Keys> keys)
    {
        _logger = logger;
        _keys = keys.Value;
    }

    public async Task<bool> ValiderSignatur(KryptertStemme kryptertStemme)
    {
        var publicKey = await GetPublicKey(kryptertStemme.FolkeavstemningId);
        var signatur = BigInteger.Parse(kryptertStemme.Signatur, NumberStyles.None);

        if (!signatur.IsSignatureLengthValid(publicKey))
        {
            return false;
        }

        var (e, n) = publicKey.GetPublicComponents();
        var mtest = BigInteger.ModPow(signatur, e, n);

        var hashBuffer = SHA384.HashData(Encoding.UTF8.GetBytes(kryptertStemme.Data));
        var str = "0" + Convert.ToHexString(hashBuffer);
        var hash = BigInteger.Parse(str, NumberStyles.HexNumber);

        return hash == mtest;
    }

    private async Task<RSA> GetPublicKey(string folkeavstemningId)
    {
        if (_rsaKeys.TryGetValue(folkeavstemningId, out var existingKey))
        {
            return existingKey;
        }

        if (!_keys.TryGetValue(folkeavstemningId.ToLinuxPortableCharacterSet(), out var keyLocations))
        {
            _logger.LogError("Mangler nøkkel-konfigurasjon for {FolkeavstemningId}", folkeavstemningId);
            throw new KeyNotFoundException($"Mangler nøkkel-konfigurasjon for {folkeavstemningId}");
        }

        var signingPublicKey = await File.ReadAllTextAsync(keyLocations.SigningPublic);

        var rsa = RSA.Create();
        rsa.ImportFromPem(signingPublicKey);
        _rsaKeys.TryAdd(folkeavstemningId, rsa);
        return rsa;
    }
}
