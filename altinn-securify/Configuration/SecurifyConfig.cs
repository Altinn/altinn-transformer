namespace Altinn.Securify.Configuration;

public class SecurifyConfig
{
    public int MaxPlainTextSizeInBytes { get; set; }
    public TimeSpan DefaultLifeTime { get; set; }
    public TimeSpan MaxLifeTime { get; set; }
    public string EncryptionKeys { get; set; } = null!;
}