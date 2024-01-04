using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Resultat.Api.Certificates;
using Resultat.Api.Crypto;
using Resultat.Api.Database;
using Shared.Configuration;
using Shared.Export;

namespace Resultat.Api.Resultat;

[ApiController]
[Tags("2. Dekrypter stemmer og regn ut resultat")]
[AllowAnonymous] // The correct private key is required, and used as authorization
public class ResultatController : ControllerBase
{
    private readonly ResultatContext _context;
    private readonly IClock _clock;
    private readonly IOptions<Keys> _keys;
    private readonly ILogger<ResultatController> _logger;
    private readonly ValidationService _validationService;

    public ResultatController(ResultatContext context, IClock clock, IOptions<Keys> keys, ILogger<ResultatController> logger, ValidationService validationService)
    {
        _context = context;
        _clock = clock;
        _keys = keys;
        _logger = logger;
        _validationService = validationService;
    }

    [HttpPost("resultat/{folkeavstemningId}")]
    public async Task<IActionResult> DekrypterStemmer(string folkeavstemningId, [FromForm] PrivateKeyData data)
    {
        var folkeavstemning = FolkeavstemningsKonfigurasjon.Get(folkeavstemningId);
        if (folkeavstemning == null)
        {
            return NotFound();
        }

        var key = await LoadRsaPrivateKey(data, _keys, folkeavstemningId);

        _logger.LogInformation("Starter dekryptering og opptelling av stemmer");

        if (folkeavstemning.Regler.AvstemningMåStengeFørResultatKanHentes && folkeavstemning.Lukker.ToInstant() > _clock.GetCurrentInstant())
        {
            _logger.LogWarning("Opptelling er forsøkt startet før avstemningsperioden er over");
            return Forbid();
        }

        var krypterteStemmer = await GetKrypterteStemmer(_context, folkeavstemningId);
        var validerteStemmer = await ValiderSignaturer(folkeavstemningId, krypterteStemmer);
        var dekrypterteStemmer = DekrypterStemmer(validerteStemmer, key);
        var stemmepakker = DeserialiserStemmer(dekrypterteStemmer);
        var resultat = LagResultat(stemmepakker);

        return File(resultat, "text/csv", $"Opptelling digitale stemmer - {folkeavstemningId}.csv");
    }

    private static async Task<KryptertStemme[]> GetKrypterteStemmer(ResultatContext context, string folkeavstemningId)
    {
        await context.Database.ExecuteSqlRawAsync("cluster krypterte_stemmer");
        var krypterteStemmer = await context.Stemmer
            .OrderBy(x=>x.Id)
            .Where(x => x.FolkeavstemningId == folkeavstemningId)
            .ToArrayAsync();

        return krypterteStemmer;
    }

    private async Task<ValidertStemme[]> ValiderSignaturer(string folkeavstemningId, KryptertStemme[] krypterteStemmer)
    {
        var kontrollAvStemmer =
            krypterteStemmer
            .Select(async stemme => new ValidertStemme()
            {
                GyldigSignatur = await _validationService.ValiderSignatur(stemme),
                Data = stemme.Data
            })
            .ToArray();

        var validerteStemmer = await Task.WhenAll(kontrollAvStemmer);

        if (validerteStemmer.Any(stemme => stemme.GyldigSignatur == false))
        {
            _logger.LogError("Ugyldig stemme funnet i opptelling for {FolkeavstemningId}", folkeavstemningId);
        }

        return validerteStemmer;
    }

    private static DekryptertStemme[] DekrypterStemmer(ValidertStemme[] krypterteStemmer, RSA privateKey)
    {
        var dekrypterteStemmer = krypterteStemmer
            .AsParallel()
            .Select(x => Decrypt(privateKey, x))
            .ToArray();
        return dekrypterteStemmer;
    }

    private static DekryptertStemme Decrypt(RSA privateKey, ValidertStemme stemme)
    {
        try
        {
            var dekryptertStemmedata = privateKey.Decrypt(Convert.FromBase64String(stemme.Data), RSAEncryptionPadding.OaepSHA384);

            return new DekryptertStemme
            {
                GyldigSignatur = stemme.GyldigSignatur,
                DekryptertStemmedata = dekryptertStemmedata
            };
        }
        catch(Exception)
        {
            return new DekryptertStemme()
            {
                GyldigSignatur = stemme.GyldigSignatur,
                DekryptertStemmedata = null
            };
        }
    }

    private static DeserialisertStemme[] DeserialiserStemmer(DekryptertStemme[] dekrypterteStemmer)
    {
        var stemmepakker = dekrypterteStemmer.Select(dekryptertStemme =>
        {
            var deserialisertStemmepakke = dekryptertStemme.DekryptertStemmedata == null ? null : JsonSerializer.Deserialize<Stemmepakke>(dekryptertStemme.DekryptertStemmedata);
            return new DeserialisertStemme()
            {
                GyldigSignatur = dekryptertStemme.GyldigSignatur,
                DekryptertStemmedata = deserialisertStemmepakke,
            };
        }).ToArray();
        return stemmepakker;
    }

    private static byte[] LagResultat(DeserialisertStemme[] stemmepakker)
    {
        var resultat = stemmepakker
            .Select(stemme =>
            {
                return stemme switch
                {
                    { GyldigSignatur: false } => "<UGYLDIG SIGNATUR>",
                    { DekryptertStemmedata: null } => "<UGYLDIG STEMME>",
                    { DekryptertStemmedata.Valg: "" } => "<TOM STEMME>",
                    { DekryptertStemmedata.Valg: null } => "<TOM STEMME>",

                    // En manuel komponert stemmepakke kunne ha overnevnt tekst i "Valg" for å skape antakelser om at det er kommet inn ugyldige stemmer
                    { DekryptertStemmedata.Valg: "<UGYLDIG SIGNATUR>" } => $"Eget valg: {stemme.DekryptertStemmedata.Valg}",
                    { DekryptertStemmedata.Valg: "<UGYLDIG STEMME>" } => $"Eget valg: {stemme.DekryptertStemmedata.Valg}",

                    { DekryptertStemmedata: var stemmedata } => stemmedata.Valg
                };
            })
            .GroupBy(x => x)
            .Select(x => new { Valg = x.Key, Stemmer = x.Count() })
            .ExportCsv();
        return Encoding.UTF8.GetBytes(resultat);
    }



    private async Task<RSA> LoadRsaPrivateKey(PrivateKeyData keyData, IOptions<Keys> keys, string folkeavstemningId)
    {
        if (!keys.Value.TryGetValue(folkeavstemningId.ToLinuxPortableCharacterSet(), out var keyLocations))
        {
            _logger.LogError("Mangler nøkkel-konfigurasjon for {FolkeavstemningId}", folkeavstemningId);
            throw new Exception("Mangler nøkkel-konfigurasjon");
        }

        await using var stream = keyData.FormFile.OpenReadStream();
        using var streamReader = new StreamReader(stream);
        var key = await streamReader.ReadToEndAsync();

        var rsa = RSA.Create();
        rsa.ImportFromPem(key);

        var publicKeyData = await System.IO.File.ReadAllTextAsync(keyLocations.EncryptionPublic);
        var publicRsa = RSA.Create();
        publicRsa.ImportFromPem(publicKeyData);

        EnsurePrivateKeyMatchingPublicKey(rsa, publicRsa);

        return rsa;
    }

    private void EnsurePrivateKeyMatchingPublicKey(RSA rsa, RSA publicRsa)
    {
        var dataToTest = RandomNumberGenerator.GetBytes(128);
        var encryptedData = publicRsa.Encrypt(dataToTest, RSAEncryptionPadding.OaepSHA384);
        try
        {
            var decryptedData = rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA384);

            if (decryptedData.SequenceEqual(dataToTest))
            {
                return;
            }
        }
        catch (CryptographicException)
        {
        }
        _logger.LogError("Forsøk på opptelling med feil valgnøkkel");
        throw new Exception("Feil valgnøkkel");
    }
}

internal class ValidertStemme
{
    public required bool GyldigSignatur { get; set; }
    public required string Data { get; set; }
}

internal class DekryptertStemme
{
    public required bool GyldigSignatur { get; set; }
    public required byte[]? DekryptertStemmedata { get; set; }
}

internal class DeserialisertStemme
{
    public required bool GyldigSignatur { get; set; }
    public required Stemmepakke? DekryptertStemmedata { get; set; }
}

public class PrivateKeyData
{
    [Required]
    public required IFormFile FormFile { get; set; }
}

public class Stemmepakke
{
    [JsonPropertyName("valg")]
    public string? Valg { get; set; }

    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }
}
