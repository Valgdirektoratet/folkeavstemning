using Shared.Configuration;

namespace Resultat.Api.Certificates;

public class Keys : Dictionary<string, KeyLocations>
{
    public bool Validate(ILogger<Keys> logger)
    {
        foreach (var folkeavstemning in FolkeavstemningsKonfigurasjon.Folkeavstemninger)
        {
            if (!ContainsKey(folkeavstemning.FolkeavstemningIdNormalized))
            {
                logger.LogError("Mangler nøkkel-konfigurasjon for {Folkeavstemning} ({FolkeavstemningNormalized}", folkeavstemning.FolkeavstemningId, folkeavstemning.FolkeavstemningIdNormalized);
                return false;
            }

            var keyLocations = this[folkeavstemning.FolkeavstemningIdNormalized];

            if (!File.Exists(keyLocations.SigningPublic))
            {
                logger.LogError("Finner ikke SigningPublic: '{Path}'", keyLocations.SigningPublic);
                return false;
            }

            if (!File.Exists(keyLocations.SigningPublic))
            {
                logger.LogError("Finner ikke EncryptionPublic: '{Path}'", keyLocations.EncryptionPublic);
                return false;
            }
        }

        return true;
    }
}

public class KeyLocations
{
    public required string SigningPublic { get; set; }
    public required string EncryptionPublic { get; set; }
}

