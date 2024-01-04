using System.Diagnostics.CodeAnalysis;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Manntall.Client;

public interface IManntallClient
{
    Task<string?> FindManntallsnummerForPerson(string folkeavstemningsId, string personIdentificationNumber);
    Task<PersonIManntall[]> FindFolkeavstemningForPerson(string personIdentificationNumber);
}

public class ManntallClient : IManntallClient
{
    private readonly ILogger<ManntallClient> _logger;
    private readonly IOptions<ManntallOptions> _options;

    public ManntallClient(ILogger<ManntallClient> logger, IOptions<ManntallOptions> options)
    {
        _logger = logger;
        _options = options;
    }
    public async Task<string?> FindManntallsnummerForPerson(string folkeavstemningsId, string personIdentificationNumber)
    {
        _logger.LogDebug("FindManntallForPerson");
        var options = _options.Value;

        var manntallsnummer = await options.Url
            .AppendPathSegments("manntall", folkeavstemningsId, "person", personIdentificationNumber)
            .WithBasicAuth(options.Username, options.Password)
            .GetJsonAsync<string?>();

        return manntallsnummer;
    }

    public async Task<PersonIManntall[]> FindFolkeavstemningForPerson(string personIdentificationNumber)
    {
        _logger.LogDebug("FindManntallForPerson");
        var options = _options.Value;

        var manntallsnummer = await options.Url
            .AppendPathSegments("person", personIdentificationNumber)
            .WithBasicAuth(options.Username, options.Password)
            .GetJsonAsync<PersonIManntallDto[]?>();

        if (manntallsnummer != null)
        {
            return manntallsnummer
                .Where(x=>x is { Manntallsnummer: not null, FolkeavstemningId: not null })
                .Select(x => new PersonIManntall() { Manntallsnummer = x.Manntallsnummer!, FolkeavstemningId = x.FolkeavstemningId! })
                .ToArray();
        }

        return Array.Empty<PersonIManntall>();
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private class PersonIManntallDto
    {
        public string? FolkeavstemningId { get; set; }
        public string? Manntallsnummer { get; set; }
    }
}

public class PersonIManntall
{
    public required string FolkeavstemningId { get; set; }
    public required string Manntallsnummer { get; set; }
}
