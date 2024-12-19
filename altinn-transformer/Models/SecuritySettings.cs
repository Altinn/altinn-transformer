namespace Altinn.Transformer.Models;

public class SecuritySettings
{
    public DateTimeOffset ExpiresAt { get; set; }
    public List<string>? RequiresOrgNo { get; set; }
    public List<string>? RequiresClientId { get; set; }
    public List<string>? RequiresScope { get; set; }
}