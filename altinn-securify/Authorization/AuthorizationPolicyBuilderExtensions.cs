﻿using Microsoft.AspNetCore.Authorization;

namespace Altinn.Securify.Authorization;

internal static class AuthorizationPolicyBuilderExtensions
{
    private const string ScopeClaim = "scope";
    private const char ScopeClaimSeparator = ' ';

    public static AuthorizationPolicyBuilder RequireScope(this AuthorizationPolicyBuilder builder, string scope) =>
        builder.RequireAssertion(ctx => ctx.User.Claims
            .Where(x => x.Type == ScopeClaim)
            .Select(x => x.Value)
            .Any(scopeValue => scopeValue == scope || scopeValue
                .Split(ScopeClaimSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Contains(scope)));

    public static AuthorizationPolicyBuilder RequireValidConsumerClaim(this AuthorizationPolicyBuilder builder) =>
        builder.RequireAssertion(ctx => ctx.User.TryGetOrganizationNumber(out _));
}
