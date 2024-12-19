using System.Text.Json;
using Altinn.Transformer.Configuration;
using Altinn.Transformer.Helpers;

namespace Altinn.Transformer.Models.Dto;

public class TransformerInputDto
{
    public Uri InputType { get; set; } = null!;
    public JsonElement InputValue { get; set; }
    public Uri RequestedOutputType { get; set; } = null!;
    public SecuritySettingsDto? SecuritySettings { get; set; } = null!;
    
    private static readonly Dictionary<TransformableType, List<TransformableType>> ValidPairings = new()
    {
        { TransformableType.UserId, [
                TransformableType.PartyId,
                TransformableType.PersonId,
                TransformableType.Secured
            ]
        },
        { TransformableType.PartyId, [
                TransformableType.UserId,
                TransformableType.PersonId,
                TransformableType.OrganizationId,
                TransformableType.PersonOrOrganizationId,
                TransformableType.Secured
            ]
        },
        { TransformableType.PersonId, [
                TransformableType.UserId,
                TransformableType.PartyId,
                TransformableType.Secured
            ]
        },
        { TransformableType.OrganizationId, [
                TransformableType.PartyId,
                TransformableType.Secured
            ]
        },
        { TransformableType.Secured, [
                TransformableType.UserId,
                TransformableType.PartyId,
                TransformableType.PersonId,
                TransformableType.OrganizationId,
                TransformableType.PersonOrOrganizationId,
                TransformableType.Raw
            ]
        },
        { TransformableType.Raw, [
                TransformableType.Secured
            ]
        }
    };
    
    public List<string> Validate(TransformerConfig transformerConfig)
    {
        var errors = new List<string>();

        var inputType = InputType.ToTransformableType();
        if (inputType == TransformableType.Unknown)
        {
            errors.Add($"Invalid InputType: {InputType}.");
        }

        var outputType = RequestedOutputType.ToTransformableType();
        if (outputType == TransformableType.Unknown)
        {
            errors.Add($"Invalid RequestedOutputType: {RequestedOutputType}.");
        }
        
        if (!ValidPairings.TryGetValue(inputType, out List<TransformableType>? value) || !value.Contains(outputType))
        {
            errors.Add($"Invalid RequestedOutputType: {RequestedOutputType} for InputType: {InputType}.");
        }
        
        if (SecuritySettings != null)
        {
            errors.AddRange(SecuritySettings.Validate(transformerConfig));
        }

        return errors;
    }

    public TransformerInput ToTransformerInput(TransformerConfig transformerConfig)
    {
        return new TransformerInput
        {
            InputType = InputType.ToTransformableType(),
            InputValue = InputValue,
            RequestedOutputType = RequestedOutputType.ToTransformableType(),
            SecuritySettings = SecuritySettings?.ToSecuritySettings(transformerConfig) 
                               ?? new SecuritySettings
                               {
                                   ExpiresAt = DateTimeOffset.UtcNow.Add(transformerConfig.DefaultSecuredDataLifeTime)
                               }
        };
    }

}