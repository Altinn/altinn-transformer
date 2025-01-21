using System.Text.Json;
using Altinn.Securify.Configuration;

namespace Altinn.Securify.Models.Dto;

public class EncryptionRequestDto
{
    public JsonElement PlainText { get; set; }
    public EncryptionSettingsDto? Settings { get; set; } = null!;
    
    public List<string> Validate(SecurifyConfig securifyConfig)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(PlainText.ToString()))
        {
            errors.Add("Cannot encrypt empty plaintext");
        }
        else if (PlainText.ToString().Length > securifyConfig.MaxPlainTextSizeInBytes)
        {
            errors.Add($"PlainText is too long. Max length is {securifyConfig.MaxPlainTextSizeInBytes} bytes.");
        }
        
        if (Settings != null)
        {
            errors.AddRange(Settings.Validate(securifyConfig));
        }

        return errors;
    }

    public EncryptionRequest ToEncryptionRequest(SecurifyConfig securifyConfig)
    {
        return new EncryptionRequest
        {
            PlainText = PlainText,
            Settings = Settings?.ToEncryptionSettings(securifyConfig) 
                               ?? new EncryptionSettings
                               {
                                   ExpiresAt = DateTimeOffset.UtcNow.Add(securifyConfig.DefaultLifeTime)
                               }
        };
    }

}