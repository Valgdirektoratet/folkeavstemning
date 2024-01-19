using IdentityModel.Client;
using Microsoft.IdentityModel.Logging;

namespace Folkeavstemning.Bff.ConfigureServices;

public static class ConfigureDuendeBff
{
    public static void AddDuendeBff(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        if (builder.Environment.IsDevelopment())
        {
            IdentityModelEventSource.ShowPII = true;
        }

        services.AddBff(options =>
            {
                options.ManagementBasePath = "/bff";
            }).AddServerSideSessions();

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignOutScheme = "oidc";
            })
            .AddCookie("cookie", options =>
            {
                // set secure policy
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                // set session lifetime
                options.ExpireTimeSpan = TimeSpan.FromMinutes(25);

                // sliding or absolute
                options.SlidingExpiration = true;

                // host prefixed cookie name
                options.Cookie.Name = "__Host-folkeavstemning";

                // strict SameSite handling
                options.Cookie.SameSite = SameSiteMode.Strict;
            })
            .AddOpenIdConnect("oidc", options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    options.RequireHttpsMetadata = false;
                }

                options.Authority = configuration["OIDC:Authority"];

                // confidential client using code flow + PKCE
                options.ClientId = configuration["OIDC:ClientId"];
                options.ClientSecret = configuration["OIDC:ClientSecret"];
                options.ResponseType = "code";
                options.RemoteSignOutPath = "/frontchannelLogout-oidc";
                // query response type is compatible with strict SameSite mode
                options.ResponseMode = "query";

                // get claims without mappings
                options.MapInboundClaims = false;
                options.GetClaimsFromUserInfoEndpoint = true;

                // save tokens into authentication session
                // to enable automatic token management
                options.SaveTokens = true;

                // request scopes
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");

                options.Events.OnAuthorizationCodeReceived = context =>
                {
                    context.Backchannel.SetBasicAuthenticationOAuth(context.TokenEndpointRequest?.ClientId ?? "", context.TokenEndpointRequest?.ClientSecret ?? "");
                    return Task.CompletedTask;
                };

            });
    }
}
