using System.Text.Json;

namespace Altinn.Transformer.Models;

public record SecuredData(SecuritySettings SecuritySettings, JsonElement Data);