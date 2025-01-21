using System.Text.Json;

namespace Altinn.Securify.Models.Dto;

public class DecryptionResultDto
{
    public JsonElement PlainText { get; set; } = new();
}
