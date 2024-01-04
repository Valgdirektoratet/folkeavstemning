using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace Manntall.Backend.Helpers;

public static class ConfigHelper
{
    public static void ConfigureIntegrationOptions(this IServiceCollection services)
    {
        services.AddOptions<MaskinportenConfig>().BindConfiguration("Maskinporten").ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<FolkeregisterConfig>().BindConfiguration("Folkeregister").ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<KrrConfig>().BindConfiguration("Krr").ValidateDataAnnotations().ValidateOnStart();
    }
}

public class FolkeregisterConfig
{
    [Required]
    public required string Url { get; init; }
}

public class KrrConfig
{
    [Required]
    public required string Url { get; init; }
}

public class MaskinportenConfig
{
    [Required]
    public required VirksomhetssertifikatConfig Virksomhetssertifikat { get; init; }

    [Required]
    public required string ClientId { get; init; }

    [Required]
    public required string Scopes { get; init; }

    [Required]
    public required string Url { get; init; }

    [Required]
    public required string Audience { get; set; }
}

public class VirksomhetssertifikatConfig
{
    public string? File { get; init; }
    public string? Password { get; init; }
    public string? StoreLocation { get; init; }
    public string? Thumbprint { get; init; }

    [NotMapped]
    public Lazy<X509Certificate2> Certificate => new(LoadCertificate);

    private X509Certificate2 LoadCertificate()
    {
        var certificate = this switch
        {
            { File: not null, Password: not null } => LoadCertificateFromFile(File, Password),
            { StoreLocation: not null, Thumbprint: not null } => LoadCertificateFromStore(StoreLocation, Thumbprint),
            _ => throw new Exception("Bad certificate configuration"),
        };
        return certificate;
    }

    private static X509Certificate2 LoadCertificateFromStore(string storeLocation, string thumbprint)
    {
        if (!Enum.TryParse<StoreLocation>(storeLocation, out var location))
        {
            throw new ArgumentException("Bad store location - allowed values: CurrentUser, LocalMachine");
        }

        var store = new X509Store(StoreName.My, location);
        store.Open(OpenFlags.ReadOnly);

        var cert = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true).FirstOrDefault();

        if (cert != null)
        {
            return cert;
        }

        throw new Exception($"Certificate with thumbprint {thumbprint} not found in {storeLocation}");
    }

    private static X509Certificate2 LoadCertificateFromFile(string file, string password) =>
        new(file, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
}
