namespace Altinn.Transformer.Models;

public class TransformationResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
    public TransformerOutput Output { get; set; } = null!;
}