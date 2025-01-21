namespace Altinn.Securify.Services.Interfaces;

public interface IKeyResolverService
{
    public Task<byte[]?> GetKey(string keyId);
}