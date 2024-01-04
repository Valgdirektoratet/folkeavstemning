using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Authentication;

public static class AuthExtensions
{
    public static void AddBaseAuthenticationEx(this WebApplicationBuilder application)
    {
        application.Services.AddAuthentication("BasicAuthentication")
            .AddScheme<BasicAuthenticationSchemeOptions, BasicAuthenticationHandler>(
                "BasicAuthentication",
                options => application.Configuration.GetSection("Authentication:Basic").Bind(options));
        application.Services.AddAuthorization();
    }
}

public class BasicAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public string? Username { get; set; }
    private string? _password;

    public string? Password
    {
        get => _password;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NullReferenceException("Password is null, empty or only whitespace");
            }

            var decodedBytes = Convert.FromBase64String(value);
            _password = System.Text.Encoding.UTF8.GetString(decodedBytes);
        }
    }
}

