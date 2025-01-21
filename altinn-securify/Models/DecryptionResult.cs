using System.Text.Json;
using Altinn.Securify.Models.Dto;

namespace Altinn.Securify.Models;

public class DecryptionResult
{
    public JsonElement PlainText { get; set; }
    public List<string> Errors { get; set; } = new();
    
    public DecryptionResultDto ToDecryptionResultDto()
    {
        return new DecryptionResultDto
        {
            PlainText = PlainText
        };
    }
}