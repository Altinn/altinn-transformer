using System.Text.RegularExpressions;
using Altinn.Securify.Configuration;

namespace Altinn.Securify.Models.Dto;

public partial class EncryptionSettingsDto
{
    public DateTimeOffset? ExpiresAt { get; set; }
    public List<string>? RequiresOrgNo { get; set; }
    public List<string>? RequiresClientId { get; set; }
    public List<string>? RequiresScope { get; set; }

    private const string ValidNorwegianOrgNoPattern = @"^\d{9}$";
    
    [GeneratedRegex(ValidNorwegianOrgNoPattern)]
    private static partial Regex ValidNorwegianOrgNoRegex();
    
    public List<string> Validate(SecurifyConfig securifyConfig)
    {
        var errors = new List<string>();

        if (ExpiresAt.HasValue && (ExpiresAt <= DateTimeOffset.UtcNow || ExpiresAt >= DateTimeOffset.UtcNow.Add(securifyConfig.MaxLifeTime)))
        {
            errors.Add($"ExpiresAt must be within {securifyConfig.MaxLifeTime.TotalSeconds} seconds in the future.");
        }
        
        if (RequiresOrgNo != null && RequiresOrgNo.Count != 0)
        {
            errors.AddRange(from orgNo in RequiresOrgNo where !ValidNorwegianOrgNoRegex().IsMatch(orgNo) 
                select $"Invalid Norwegian organization number: {orgNo}");
        }

        return errors;
    }

    public EncryptionSettings ToEncryptionSettings(SecurifyConfig securifyConfig)
    {
        return new EncryptionSettings
        {
            ExpiresAt = ExpiresAt ?? DateTimeOffset.UtcNow.Add(securifyConfig.DefaultLifeTime),
            RequiresOrgNo = RequiresOrgNo ?? [],
            RequiresClientId = RequiresClientId ?? [],
            RequiresScope = RequiresScope ?? []
        };
    }

}