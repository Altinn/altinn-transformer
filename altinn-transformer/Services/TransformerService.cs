using System.Text.Json;
using System.Buffers.Text;
using System.Text;
using Altinn.Transformer.Models;
using Altinn.Transformer.Services.Interfaces;

namespace Altinn.Transformer.Services;

public class TransformerService : ITransformerService
{
    private const string CurrentKeyId = "20250102";
    
    private readonly IEncryptionService _encryptionService;
    private readonly IKeyResolverService _keyResolverService;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public TransformerService(IEncryptionService encryptionService, IKeyResolverService keyResolverService, IHttpContextAccessor httpContextAccessor)
    {
        _encryptionService = encryptionService;
        _keyResolverService = keyResolverService;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<TransformationResult> Transform(TransformerInput input)
    {
        if (input.RequestedOutputType == TransformableType.Secured)
        {
            return await GetEncryptionTransformationResult(input);
        }
        
        if (input.InputType == TransformableType.Secured)
        {
            return await GetDecryptionTransformationResult(input);
        }
        
        // TODO! Implement transformation from/to other types, eg. PartyId, UserId
        throw new NotImplementedException();
    }

    private async Task<TransformationResult> GetDecryptionTransformationResult(TransformerInput input)
    {
        var securedData = await GetDecodedAndDecryptedData(input);
        var errors = ValidateSecuredData(securedData, input.RequestedOutputType);
            
        if (errors.Count > 0)
        {
            return new TransformationResult
            {
                Success = false,
                Errors = errors
            };
        }
            
        return new TransformationResult
        {
            Success = true,
            Output = new TransformerOutput
            {
                OutputType = input.RequestedOutputType,
                OutputValue = securedData.Data
            }
        };
    }

    private async Task<TransformationResult> GetEncryptionTransformationResult(TransformerInput input)
    {
        var encodedAndEncryptedData = await GetEncodedAndEncryptedData(input);

        return new TransformationResult
        {
            Success = true,
            Output = new TransformerOutput
            {
                OutputType = input.RequestedOutputType,
                OutputValue = encodedAndEncryptedData
            }
        };
    }

    private List<string> ValidateSecuredData(SecuredData securedData, TransformableType requestedOutputType)
    {
        var errors = new List<string>();
        if (securedData.SecuritySettings.ExpiresAt < DateTimeOffset.UtcNow)
        {
            errors.Add("Secured data has expired");
        }
        
        // TODO! Check HTTP context for token claims
        // TODO! Check if output data is valid for the requested output type (if not Raw)
        return errors;
    }

    private async Task<string> GetEncodedAndEncryptedData(TransformerInput input)
    {
        var plaintext = JsonSerializer.SerializeToUtf8Bytes(new SecuredData(input.SecuritySettings, input.InputValue));
        var ciphertext = await _encryptionService.Encrypt(plaintext, CurrentKeyId, _keyResolverService.GetKey);
        return Base64Url.EncodeToString(ciphertext);
    }
    
    private async Task<SecuredData> GetDecodedAndDecryptedData(TransformerInput input)
    {
        var ciphertext = Base64Url.DecodeFromUtf8(Encoding.UTF8.GetBytes(input.InputValue.GetString() ?? throw new InvalidOperationException("InputValue is not a valid string")));
        var plaintext = await _encryptionService.Decrypt(ciphertext, _keyResolverService.GetKey);

        var securedData = JsonSerializer.Deserialize<SecuredData>(plaintext) 
                          ?? throw new InvalidOperationException("Decrypted data could not be deserialized into SecuredData");

        return securedData;
    }
}