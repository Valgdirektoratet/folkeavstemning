using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Shared.Authentication;

public static class SwaggerExtensions
{
    public static void AddSwaggerEx(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.OrderActionsBy(description =>
            {
                var first = description.ActionDescriptor.EndpointMetadata.OfType<TagsAttribute>().First();
                var firstOrDefault = first.Tags.FirstOrDefault();
                return firstOrDefault;
            });
            c.AddSecurityDefinition("basic",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "basic authentication"
                });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "basic" } },
                    Array.Empty<string>()
                }
            });
        });
    }

    public static void UseSwaggerEx(this WebApplication app, string documentTitle)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.DocumentTitle = documentTitle;
            options.RoutePrefix = "";
            options.SwaggerEndpoint("swagger/v1/swagger.json", "API");
            options.EnableTryItOutByDefault();
            if (app.Environment.IsDevelopment())
            {
                options.EnablePersistAuthorization();
            }
        });
    }
}
