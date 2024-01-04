using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Manntall.Backend.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Manntall.Backend.Maskinporten;

public class MaskinportenClient
{
    private readonly ILogger<MaskinportenClient> _logger;
    private readonly HttpClient _client;
    private readonly IOptions<MaskinportenConfig> _config;

    public MaskinportenClient(ILogger<MaskinportenClient> logger, HttpClient client, IOptions<MaskinportenConfig> config)
    {
        ArgumentNullException.ThrowIfNull(config);

        _logger = logger;
        _client = client;
        _config = config;
    }

    private MaskinportenToken? Token { get; set; }

    public async ValueTask<MaskinportenToken> GetMaskinportenToken()
    {
        if (Token?.IsValid() != true)
        {
            _logger.LogDebug("Obtaining new Maskinporten token");
            Token = await GetNewToken();
        }
        else
        {
            _logger.LogDebug("Reusing Maskinporten token");
        }

        _logger.LogDebug("Using Maskinporten token with expiration time of {ExpirationTime}", Token.ExpirationTime);
        return Token;
    }

    private async Task<MaskinportenToken> GetNewToken()
    {
        var assertion = GetJwtAssertion();
        var grant = new Dictionary<string, string?>
        {
            ["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer",
            ["assertion"] = assertion
        };

        var content = new FormUrlEncodedContent(grant);
        try
        {
            var result = await _client.PostAsync(_config.Value.Url, content);
            result.EnsureSuccessStatusCode();
            var token = await result.Content.ReadFromJsonAsync<AccessTokenResponse>();
            ArgumentException.ThrowIfNullOrEmpty(token?.AccessToken);
            return new MaskinportenToken(token.AccessToken, DateTimeOffset.Now.AddSeconds(token.ExpiresIn));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to obtain Maskinporten token");
            throw;
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private class AccessTokenResponse
    {
        // An Oauth2 access token, either by reference or as a JWT depending
        // on which scopes was requested and/or client registration properties.
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }

    private string GetJwtAssertion()
    {
        var securityKey = new X509SecurityKey(_config.Value.Virksomhetssertifikat.Certificate.Value);
        var certificateChain = new[] { Convert.ToBase64String(_config.Value.Virksomhetssertifikat.Certificate.Value.GetRawCertData()) };

        var header = new JwtHeader(new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256)) { { "x5c", certificateChain } };
        header.Remove("typ");
        header.Remove("kid");

        var issuedAt = DateTimeOffset.UtcNow;
        var expirationTime = issuedAt.AddSeconds(120);

        var body = new JwtPayload
        {
            { "aud", _config.Value.Audience },
            { "scope", _config.Value.Scopes },
            { "iss", _config.Value.ClientId },
            { "exp", expirationTime.ToUnixTimeSeconds() },
            { "iat", issuedAt.ToUnixTimeSeconds() },
            { "jti", Guid.NewGuid().ToString() }
        };
        var securityToken = new JwtSecurityToken(header, body);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(securityToken);
        return token;
    }
}



public record MaskinportenToken(string Token, DateTimeOffset ExpirationTime)
{
    public bool IsValid() => ExpirationTime > DateTimeOffset.UtcNow.AddSeconds(5); // add some delay
}

public class AuthHandler : DelegatingHandler
{
    private readonly MaskinportenClient _maskinportenClient;

    public AuthHandler(MaskinportenClient maskinportenClient)
    {
        _maskinportenClient = maskinportenClient;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var (token, _) = await _maskinportenClient.GetMaskinportenToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
