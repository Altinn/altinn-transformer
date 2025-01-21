using System.Text.Json;

namespace Altinn.Securify.Models;

public class EncryptionRequest
{
    public JsonElement PlainText { get; set; }
    public EncryptionSettings Settings { get; set; } = new();
}