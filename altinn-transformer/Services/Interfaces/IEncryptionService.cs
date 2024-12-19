namespace Altinn.Transformer.Services.Interfaces;

public interface IEncryptionService
{
    public Task<byte[]> Encrypt(byte[] plainText, string keyId, Func<string, Task<byte[]?>> keyResolver);
    public Task<byte[]> Decrypt(byte[] cipherText, Func<string, Task<byte[]?>> keyResolver);
}