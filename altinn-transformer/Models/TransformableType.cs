namespace Altinn.Transformer.Models;

public enum TransformableType
{
    Unknown,
    PartyId,
    UserId,
    PersonId,
    OrganizationId,
    PersonOrOrganizationId,
    Raw,
    Secured
}