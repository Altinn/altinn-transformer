using Altinn.Securify.Models.Dto;

namespace Altinn.Securify.Models;

public class EncryptionResult
{
    public string CipherText { get; set; } = null!;
    
    public EncyptionResultDto ToEncryptionResultDto()
    {
        return new EncyptionResultDto
        {
            CipherText = CipherText
        };
    }
}