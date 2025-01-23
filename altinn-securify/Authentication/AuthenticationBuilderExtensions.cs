using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using Altinn.Securify.Configuration;

namespace Altinn.Securify.Authentication;

internal static class AuthenticationBuilderExtensions
{
    public static IServiceCollection AddSecurifyAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtTokenSchemas = configuration
            .GetSection(nameof(SecurifyConfig))
            .Get<SecurifyConfig>()
            ?.Authentication
            ?.JwtBearerTokenSchemas;

        if (jwtTokenSchemas is null || jwtTokenSchemas.Count == 0)
        {
            throw new InvalidOperationException("No JWT token schemas found in configuration.");
        }

        services.AddSingleton<ITokenIssuerCache, TokenIssuerCache>();

        var authenticationBuilder = services.AddAuthentication();

        foreach (var schema in jwtTokenSchemas)
        {
            authenticationBuilder.AddJwtBearer(schema.Name, options =>
            {
                options.MetadataAddress = schema.WellKnown;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(2)
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = async context =>
                    {
                        var expectedIssuer = await context.HttpContext
                            .RequestServices
                            .GetRequiredService<ITokenIssuerCache>()
                            .GetIssuerForScheme(schema.Name);

                        if (context.HttpContext.Items.TryGetValue(Constants.CurrentTokenIssuer, out var tokenIssuer)
                            && (string?)tokenIssuer != expectedIssuer)
                        {
                            context.NoResult();
                        }
                    }
                };
            });
        }

        return services;
    }
}