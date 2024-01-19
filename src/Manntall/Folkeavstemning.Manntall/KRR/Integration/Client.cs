using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Manntall.Backend.Helpers;
using Manntall.Backend.Maskinporten;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Manntall.Backend.KRR.Integration;

public class KrrClient
{
    private readonly HttpClient _httpClient;

    private readonly MaskinportenClient _maskinportenClient;

    public KrrClient(IOptions<KrrConfig> config, HttpClient httpClient, MaskinportenClient maskinportenClient)
    {
        _httpClient = httpClient;
        _maskinportenClient = maskinportenClient;
        BaseUrl = config.Value.Url;
    }

    public string BaseUrl { get; set; }

    public Task<GetUsersResource> GetPersons(string[] body, CancellationToken cancellationToken) =>
        GetPersons(new GetUsersRequestV2 { Personidentifikatorer = body, InkluderIkkeRegistrerte = false }, cancellationToken);

    private async Task<GetUsersResource> GetPersons(GetUsersRequestV2 body, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(body);

        using var request = new HttpRequestMessage();
        var json = JsonConvert.SerializeObject(body);
        var content = new StringContent(json);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        request.Content = content;
        request.Method = new HttpMethod("POST");
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

        ArgumentNullException.ThrowIfNull(_maskinportenClient);
        var (token, _) = await _maskinportenClient.GetMaskinportenToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        request.RequestUri = new Uri(new Uri(BaseUrl), "/rest/v2/personer");

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        try
        {
            var headers = response.Headers.ToDictionary(h => h.Key, h => h.Value);
            foreach (var item in response.Content.Headers)
            {
                headers[item.Key] = item.Value;
            }

            var status = (int)response.StatusCode;
            switch (status)
            {
                case 200:
                    {
                        var objectResponse = await ReadObjectResponseAsync<GetUsersResource>(response, headers, cancellationToken)
                            .ConfigureAwait(false);
                        if (objectResponse.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                        }

                        return objectResponse.Object;
                    }
                case 400:
                    {
                        var responseText = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        throw new ApiException("Forbidden", status, responseText, headers, null);
                    }
                case 401:
                    {
                        var responseText = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        throw new ApiException("Unauthorized", status, responseText, headers, null);
                    }
                case 403:
                    {
                        var responseText = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        throw new ApiException("Forbidden", status, responseText, headers, null);
                    }
                case 500:
                    {
                        var responseText = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        throw new ApiException("Internal Server Error", status, responseText, headers, null);
                    }
                default:
                    {
                        var responseData = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData,
                            headers, null);
                    }
            }
        }
        finally
        {
            response.Dispose();
        }
    }

    protected virtual async Task<ObjectResponseResult<T?>> ReadObjectResponseAsync<T>(HttpResponseMessage response,
        IReadOnlyDictionary<string, IEnumerable<string>> headers, CancellationToken cancellationToken)
    {
        try
        {
            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var streamReader = new StreamReader(responseStream);
            await using var jsonTextReader = new JsonTextReader(streamReader);

            var serializer = JsonSerializer.Create();
            var typedBody = serializer.Deserialize<T>(jsonTextReader);
            return new ObjectResponseResult<T?>(typedBody, string.Empty);
        }
        catch (JsonException exception)
        {
            var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
            throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
        }
    }

    protected struct ObjectResponseResult<T>
    {
        public ObjectResponseResult(T responseObject, string responseText)
        {
            Object = responseObject;
            Text = responseText;
        }

        public T Object { get; }

        public string Text { get; }
    }
}

public class ApiException : Exception
{
    public ApiException(string message, int statusCode, string? response, IReadOnlyDictionary<string, IEnumerable<string>> headers,
        Exception? innerException)
        : base(
            message + "\n\nStatus: " + statusCode + "\nResponse: \n" +
            (response == null ? "(null)" : response[..(response.Length >= 512 ? 512 : response.Length)]), innerException)
    {
        StatusCode = statusCode;
        Response = response;
        Headers = headers;
    }

    public int StatusCode { get; private set; }

    public string? Response { get; }

    public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; }

    public override string ToString() => $"HTTP Response: \n\n{Response}\n\n{base.ToString()}";
}

public class GetUsersResource
{
    [JsonProperty("personer", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public ICollection<UserResource>? Personer { get; set; }
}

public class UserResource
{
    [JsonProperty("personidentifikator", Required = Required.Always)]
    [Required(AllowEmptyStrings = true)]
    public string Personidentifikator { get; set; } = default!;

    [JsonProperty("reservasjon", Required = Required.DisallowNull)]
    [Required(AllowEmptyStrings = true)]
    [JsonConverter(typeof(StringEnumConverter))]
    public UserResourceReservasjon Reservasjon { get; set; }

    [JsonProperty("status", Required = Required.Always)]
    [Required(AllowEmptyStrings = true)]
    [JsonConverter(typeof(StringEnumConverter))]
    public UserResourceStatus Status { get; set; }

    [JsonProperty("varslingsstatus", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(StringEnumConverter))]
    public UserResourceVarslingsstatus Varslingsstatus { get; set; }

    [JsonProperty("kontaktinformasjon", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public ContactInfoResource Kontaktinformasjon { get; set; } = default!;

    [JsonProperty("spraak", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string Språk { get; set; } = default!;
}

public class GetUsersRequestV2
{
    [JsonProperty("personidentifikatorer", Required = Required.Always)]
    [Required]
    [MinLength(1)]
    [MaxLength(1000)]
    public ICollection<string> Personidentifikatorer { get; set; } = new Collection<string>();

    [JsonProperty("inkluderIkkeRegistrerte", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public bool InkluderIkkeRegistrerte { get; set; }

}

public enum UserResourceStatus
{
    [EnumMember(Value = "AKTIV")] Aktiv = 0,

    [EnumMember(Value = "SLETTET")] Slettet = 1,

    [EnumMember(Value = "IKKE_REGISTRERT")]
    IkkeRegistrert = 2
}

public enum UserResourceReservasjon
{
    [EnumMember(Value = "JA")] Ja = 0,

    [EnumMember(Value = "NEI")] Nei = 1
}

public class ContactInfoResource
{
    [JsonProperty("epostadresse", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string Epostadresse { get; set; } = default!;

    [JsonProperty("mobiltelefonnummer", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string Mobiltelefonnummer { get; set; } = default!;
}

public enum UserResourceVarslingsstatus
{
    [EnumMember(Value = "KAN_IKKE_VARSLES")]
    KanIkkeVarsles = 0,

    [EnumMember(Value = "KAN_VARSLES")] KanVarsles = 1
}
