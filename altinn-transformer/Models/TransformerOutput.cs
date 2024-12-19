using Altinn.Transformer.Helpers;
using Altinn.Transformer.Models.Dto;

namespace Altinn.Transformer.Models;

public class TransformerOutput
{
    public TransformableType OutputType { get; set; }
    public object OutputValue { get; set; } = null!;

    public TransformerOutputDto ToTransformerOutputDto()
    {
        return new TransformerOutputDto
        {
            OutputType = OutputType.ToUrn(),
            OutputValue = OutputValue
        };
    }
}