namespace Altinn.Transformer.Models.Dto;

public class TransformerOutputDto
{
    public Uri OutputType { get; set; } = null!;
    public object OutputValue { get; set; } = null!;
}