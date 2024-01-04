using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Shared.Authentication;

public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder,
#pragma warning disable CS0618 // Type or member is obsolete
        ISystemClock clock) : base(options, logger, encoder, clock)
#pragma warning restore CS0618 // Type or member is obsolete
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            Request.Headers.TryGetValue("Authorization", out var authHeaderValue);

            var authHeader = AuthenticationHeaderValue.Parse(authHeaderValue.ToString());
            if (authHeader.Parameter != null)
            {
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');
                var username = credentials.FirstOrDefault();
                var password = credentials.LastOrDefault();
                
                var verified = BCrypt.Net.BCrypt.EnhancedVerify(password, Options.Password);
                
                if (username == Options.Username && verified)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, username ?? "Anonymous user"), new Claim(ClaimTypes.Role, "Admin")};
                    var identity = new ClaimsIdentity(claims, "basic");
                    var principal = new GenericPrincipal(identity, new []{"Admin"});
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
            }
        }
        catch (Exception ex)
        {
            return Task.FromResult(AuthenticateResult.Fail($"Authentication failed: {ex.Message}"));
        }

        return Task.FromResult(AuthenticateResult.Fail("Invalid credentials"));
    }
}
