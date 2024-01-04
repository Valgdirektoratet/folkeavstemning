using System.Net.Http.Headers;
using System.Text;
using Manntall.Backend.Helpers;
using Manntall.Backend.Maskinporten;
using Microsoft.Extensions.Options;

// ReSharper disable UnusedParameter.Local

namespace Manntall.Backend.Folkeregister.Integration;

public partial class FolkeregisterClient : IFolkeregisterClient
{
    private readonly MaskinportenClient? _maskinportenClient;

    public FolkeregisterClient(HttpClient httpClient, MaskinportenClient maskinportenClient, IOptions<FolkeregisterConfig> config): this(httpClient)
    {
        _maskinportenClient = maskinportenClient;
        BaseUrl = config.Value.Url;
    }

    private async Task PrepareRequestAsync(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_maskinportenClient);
        var (token, _) = await _maskinportenClient.GetMaskinportenToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private Task PrepareRequestAsync(HttpClient client, HttpRequestMessage request, string urlBuilder, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private Task ProcessResponseAsync(HttpClient client, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
