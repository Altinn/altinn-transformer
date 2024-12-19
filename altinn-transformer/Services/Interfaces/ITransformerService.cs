using Altinn.Transformer.Models;

namespace Altinn.Transformer.Services.Interfaces;

public interface ITransformerService
{
    public Task<TransformationResult> Transform(TransformerInput input);
}