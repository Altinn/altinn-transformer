using Altinn.Securify.Configuration;
using Altinn.Securify.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Altinn.Securify.Services;

public class SettingsBasedKeyResolverService : IKeyResolverService
{
    private readonly Dictionary<string, byte[]> _keyStore;

    public SettingsBasedKeyResolverService(IOptions<SecurifyConfig> securifyConfig)
    {
        _keyStore = securifyConfig.Value.EncryptionKeys.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(pair => pair.Split(':'))
            .ToDictionary(parts => parts[0], parts => Convert.FromBase64String(parts[1]));
    }
    
    public async Task<byte[]?> GetKey(string keyId)
    {
        _keyStore.TryGetValue(keyId, out var key);
        return await Task.FromResult(key);
    }
}