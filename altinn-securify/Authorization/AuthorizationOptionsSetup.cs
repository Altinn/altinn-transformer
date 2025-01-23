using Altinn.Securify.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Altinn.Securify.Authorization;

internal sealed class AuthorizationOptionsSetup : IConfigureOptions<AuthorizationOptions>
{
    private readonly SecurifyConfig _options;

    public AuthorizationOptionsSetup(IOptions<SecurifyConfig> options)
    {
        _options = options.Value;
    }

    public void Configure(AuthorizationOptions options)
    {
        var authenticationSchemas = _options
            .Authentication
            .JwtBearerTokenSchemas
            .Select(x => x.Name)
            .ToArray();

        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes(authenticationSchemas)
            .RequireValidConsumerClaim()
            .RequireScope(_options.RequiredScope)
            .Build();
    }
}
