using System.Text.Json;

namespace Altinn.Transformer.Models;

public class TransformerInput
{
    public TransformableType InputType { get; set; }
    public JsonElement InputValue  { get; set; }
    public TransformableType RequestedOutputType { get; set; }
    public SecuritySettings SecuritySettings { get; set; } = null!;
}