using System.Text.RegularExpressions;
using Altinn.Transformer.Configuration;

namespace Altinn.Transformer.Models.Dto;

public partial class SecuritySettingsDto
{
    public DateTimeOffset? ExpiresAt { get; set; }
    public List<string>? RequiresOrgNo { get; set; }
    public List<string>? RequiresClientId { get; set; }
    public List<string>? RequiresScope { get; set; }

    private const string ValidNorwegianOrgNoPattern = @"^\d{9}$";
    
    [GeneratedRegex(ValidNorwegianOrgNoPattern)]
    private static partial Regex ValidNorwegianOrgNoRegex();
    
    public List<string> Validate(TransformerConfig transformerConfig)
    {
        var errors = new List<string>();

        if (ExpiresAt.HasValue && (ExpiresAt <= DateTimeOffset.UtcNow || ExpiresAt >= DateTimeOffset.UtcNow.Add(transformerConfig.MaxSecuredDataLifeTime)))
        {
            errors.Add($"ExpiresAt must be within {transformerConfig.MaxSecuredDataLifeTime.TotalSeconds} seconds in the future.");
        }
        
        if (RequiresOrgNo != null && RequiresOrgNo.Count != 0)
        {
            errors.AddRange(from orgNo in RequiresOrgNo where !ValidNorwegianOrgNoRegex().IsMatch(orgNo) 
                select $"Invalid Norwegian organization number: {orgNo}");
        }

        return errors;
    }

    public SecuritySettings ToSecuritySettings(TransformerConfig transformerConfig)
    {
        return new SecuritySettings
        {
            ExpiresAt = ExpiresAt ?? DateTimeOffset.UtcNow.Add(transformerConfig.DefaultSecuredDataLifeTime),
            RequiresOrgNo = RequiresOrgNo ?? [],
            RequiresClientId = RequiresClientId ?? [],
            RequiresScope = RequiresScope ?? []
        };
    }

}