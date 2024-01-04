using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Shared.Authentication;

namespace Folkeavstemning.Api.ConfigureServices;

public static class ConfigureAuth
{
    public static void AddCustomAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication().AddJwtBearer(options =>
        {
            options.Challenge = JwtBearerDefaults.AuthenticationScheme;

            #if DEBUG
            options.RequireHttpsMetadata = false;
            #endif

            options.Authority = configuration["OIDC:Authority"];

            options.TokenValidationParameters = new()
            {
                ValidIssuer = configuration["OIDC:Authority"],
                ValidateAudience = false, // why?
                NameClaimType = "pid"
            };
        })
        .AddScheme<BasicAuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", options => configuration.GetSection("Authentication:Basic").Bind(options));
        services.AddAuthorization(options =>
        {
            var defaultAuthorizationPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
            options.DefaultPolicy = defaultAuthorizationPolicy;

            var basicAuth = new AuthorizationPolicyBuilder("BasicAuthentication").RequireAuthenticatedUser().Build();
            options.AddPolicy("BasicAuthentication", basicAuth);
        });
    }
}
