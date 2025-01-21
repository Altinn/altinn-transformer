namespace Altinn.Securify.Models.Dto;

public class DecryptionRequestDto
{
    public string CipherText { get; set; } = null!;
    
    public DecryptionRequest ToDecryptionRequest()
    {
        return new DecryptionRequest
        {
            CipherText = CipherText
        };
    }
}