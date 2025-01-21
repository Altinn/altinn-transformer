using Altinn.Securify.Models;

namespace Altinn.Securify.Services.Interfaces;

public interface ISecurifyService
{
    public Task<EncryptionResult> Encrypt(EncryptionRequest request);
    public Task<DecryptionResult> Decrypt(DecryptionRequest input);
}