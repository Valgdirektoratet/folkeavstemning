using Shared.Configuration;

namespace Folkeavstemning.Api.Certificates;

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

            if (string.IsNullOrWhiteSpace(keyLocations.SigningPrivatePassword))
            {
                logger.LogError("Passord til privat signeringsnøkkkel er ikke definert");
                return false;
            }

            if (!File.Exists(keyLocations.EncryptionPublic))
            {
                logger.LogError("Finner ikke EncryptionPublic: '{Path}'", keyLocations.EncryptionPublic);
                return false;
            }
            if(!File.Exists(keyLocations.SigningPrivate))
            {
                logger.LogError("Finner ikke SigningPrivate: '{Path}'", keyLocations.SigningPrivate);
                return false;
            }
        }

        return true;
    }
}

public class KeyLocations
{
    public required string SigningPrivate { get; set; }

    public required string SigningPrivatePassword { get; set; }

    public required string EncryptionPublic { get; set; }
}
