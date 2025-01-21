using System.Text.Json;

namespace Altinn.Securify.Models;

public record EncryptedData(EncryptionSettings Settings, JsonElement Data);