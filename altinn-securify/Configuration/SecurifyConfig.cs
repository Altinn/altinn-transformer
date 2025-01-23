using Altinn.Securify.Authentication;

namespace Altinn.Securify.Configuration;

public class SecurifyConfig
{
    public int MaxPlainTextSizeInBytes { get; set; }
    public TimeSpan DefaultLifeTime { get; set; }
    public TimeSpan MaxLifeTime { get; set; }
    public string EncryptionKeys { get; set; } = null!;
    public string RequiredScope { get; set; } = null!;
    public AuthenticationOptions Authentication { get; set; } = null!;
}