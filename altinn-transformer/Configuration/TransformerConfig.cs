namespace Altinn.Transformer.Configuration;

public class TransformerConfig
{
    public TimeSpan DefaultSecuredDataLifeTime { get; set; }
    public TimeSpan MaxSecuredDataLifeTime { get; set; }
    public string EncryptionKeys { get; set; } = null!;
}