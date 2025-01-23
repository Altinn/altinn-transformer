using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json;

namespace Altinn.Securify.Authorization;

public static class ClaimsPrincipalExtensions
{
    private const string ConsumerClaim = "consumer";
    private const string AuthorityClaim = "authority";
    private const string AuthorityValue = "iso6523-actorid-upis";
    private const string IdClaim = "ID";
    private const char IdDelimiter = ':';
    private const string IdPrefix = "0192";
    private const string ScopeClaim = "scope";
    private const char ScopeClaimSeparator = ' ';

    public static bool TryGetClaimValue(this ClaimsPrincipal claimsPrincipal, string claimType,
        [NotNullWhen(true)] out string? value)
    {
        value = claimsPrincipal.FindFirst(claimType)?.Value;
        return value is not null;
    }

    public static bool TryGetOrganizationNumber(this ClaimsPrincipal claimsPrincipal, [NotNullWhen(true)] out string? orgNumber)
        => claimsPrincipal.FindFirst(ConsumerClaim).TryGetOrganizationNumber(out orgNumber);

    public static bool HasScope(this ClaimsPrincipal claimsPrincipal, string scope) =>
        claimsPrincipal.TryGetClaimValue(ScopeClaim, out var scopes) &&
        scopes.Split(ScopeClaimSeparator).Contains(scope);

    public static bool TryGetOrganizationNumber(this Claim? consumerClaim, [NotNullWhen(true)] out string? orgNumber)
    {
        orgNumber = null;
        if (consumerClaim is null || consumerClaim.Type != ConsumerClaim)
        {
            return false;
        }

        var consumerClaimJson = JsonSerializer.Deserialize<Dictionary<string, string>>(consumerClaim.Value);

        if (consumerClaimJson is null)
        {
            return false;
        }

        if (!consumerClaimJson.TryGetValue(AuthorityClaim, out var authority) ||
            !string.Equals(authority, AuthorityValue, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!consumerClaimJson.TryGetValue(IdClaim, out var id))
        {
            return false;
        }

        orgNumber = id.Split(IdDelimiter) switch
        {
        [IdPrefix, var on] => NorwegianOrganizationIdentifier.IsValid(on) ? on : null,
            _ => null
        };

        return orgNumber is not null;
    }
}

file sealed record NorwegianOrganizationIdentifier
{
    public static bool IsValid(string value)
    {
        return value.Length == 9;
    }
}