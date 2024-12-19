using Altinn.Transformer.Models;

namespace Altinn.Transformer.Helpers;

public static class UrnToInputTypeMapper
{
    public static TransformableType ToTransformableType(this Uri inputTypeUrn)
    {
        return inputTypeUrn.AbsoluteUri switch 
        {
            "urn:altinn:party-id" => TransformableType.PartyId,
            "urn:altinn:user-id" => TransformableType.UserId,
            "urn:altinn:person:identifier-no" => TransformableType.PersonId,
            "urn:altinn:organization:identifier-no" => TransformableType.OrganizationId,
            "urn:altinn:any:identifier-no" => TransformableType.PersonOrOrganizationId,
            "urn:altinn:secured-data" => TransformableType.Secured,
            "urn:altinn:raw-value" => TransformableType.Raw,
            _ => TransformableType.Unknown
        };
    }
    
    public static Uri ToUrn(this TransformableType transformableType)
    {
        return transformableType switch
        {
            TransformableType.PartyId => new Uri("urn:altinn:party-id"),
            TransformableType.UserId => new Uri("urn:altinn:user-id"),
            TransformableType.PersonId => new Uri("urn:altinn:person:identifier-no"),
            TransformableType.OrganizationId => new Uri("urn:altinn:organization:identifier-no"),
            TransformableType.PersonOrOrganizationId => new Uri("urn:altinn:any:identifier-no"),
            TransformableType.Secured => new Uri("urn:altinn:secured-data"),
            TransformableType.Raw => new Uri("urn:altinn:raw-value"),
            _ => throw new ArgumentOutOfRangeException(nameof(transformableType), transformableType, null)
        };
    }
}