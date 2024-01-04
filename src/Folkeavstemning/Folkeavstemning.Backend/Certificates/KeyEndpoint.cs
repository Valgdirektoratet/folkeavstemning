using System.Numerics;
using System.Security.Cryptography;
using Manntall.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace Folkeavstemning.Api.Certificates;

[Route("api/keys")]
[ApiController]
[Authorize]
[EndpointGroupName("Folkeavstemning")]
public class KeyEndpoint : ControllerBase
{
    private readonly IManntallClient _client;
    private readonly IOptions<Keys> _keys;
    private readonly ILogger<KeyEndpoint> _logger;

    public KeyEndpoint(IManntallClient client, IOptions<Keys> keys, ILogger<KeyEndpoint> logger)
    {
        _client = client;
        _keys = keys;
        _logger = logger;
    }

    /// <summary>
    /// Henter public krypteringssertifikat for valget
    /// </summary>
    /// <param name="folkeavstemningId"></param>
    /// <returns></returns>
    /// <response code="401">Brukeren er ikke autorisert</response>
    /// <response code="403">Har ikke stemmerett</response>
    /// <response code="404">Ugyldig folkeavstemning</response>
    /// <response code="500">Manglende nøkkel-konfigurasjon</response>
    [HttpGet("{folkeavstemningId}/encryption/public")]
    [ProducesResponseType(typeof(VoteKeys), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<VoteKeys>> GetKeys(string folkeavstemningId)
    {
        if (!FolkeavstemningsKonfigurasjon.Exists(folkeavstemningId))
        {
            return NotFound();
        }

        if (User.Identity is { Name: not null })
        {
            var manntallsnummer = await _client.FindManntallsnummerForPerson(folkeavstemningId, User.Identity.Name);
            if (manntallsnummer == null) // require stemmerett i folkeavstemning
            {
                return Forbid();
            }
        }

        if (!_keys.Value.TryGetValue(folkeavstemningId.ToLinuxPortableCharacterSet(), out var keys))
        {
            _logger.LogError("Mangler nøkkel-konfigurasjon for {FolkeavstemningId}", folkeavstemningId);
            return Problem("Mangler nøkkel-konfigurasjon");
        }

        var encryptionPublicKey = await System.IO.File.ReadAllTextAsync(keys.EncryptionPublic);
        var signingPrivateKey = await System.IO.File.ReadAllTextAsync(keys.SigningPrivate);

        var signingKey = GetPublicSigningKeyParts(signingPrivateKey, keys.SigningPrivatePassword);

        return new VoteKeys { EncryptionPublicKey = encryptionPublicKey, E = signingKey.E, N = signingKey.N };
    }

    private static (string E, string N) GetPublicSigningKeyParts(string signingPrivateKey, string singingKeyPassword)
    {
        var rsa = RSA.Create();
        rsa.ImportFromEncryptedPem(signingPrivateKey, singingKeyPassword);
        var parameters = rsa.ExportParameters(false);

        var e = new BigInteger(parameters.Exponent!, true, true);
        var n = new BigInteger(parameters.Modulus!, true, true);

        return (e.ToString(), n.ToString());
    }
}

public class VoteKeys
{
    public required string EncryptionPublicKey { get; set; }
    public required string E { get; set; }
    public required string N { get; set; }
}
