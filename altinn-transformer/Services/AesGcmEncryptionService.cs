using System.Security.Cryptography;
using Altinn.Transformer.Services.Interfaces;

namespace Altinn.Transformer.Services;

/// <summary>
/// Provides methods for encrypting and decrypting data using AES-GCM with key rotation support.
/// </summary>
public class AesGcmEncryptionService : IEncryptionService
{
    private const int NonceSize = 12;     // 96-bit nonce
    private const int TagSize = 16;       // 128-bit authentication tag
    private const int KeySize = 32;       // 256-bit key for AES-GCM
    private const byte CurrentVersion = 1; // Format version
    private const int MaxPlaintextSize = 1024 * 2; // 2KB limit
    private const int MaxKeyIdLength = 16;  // Maximum length for key ID

    /// <summary>
    /// Encrypts data using AES-GCM with key rotation support.
    /// </summary>
    /// <param name="plainText">The data to encrypt.</param>
    /// <param name="keyId">Identifier for the key to use for encryption.</param>
    /// <param name="keyResolver">Function to retrieve the encryption key based on key ID.</param>
    /// <returns>Encrypted data including version, keyId, nonce, and authentication tag.</returns>
    public async Task<byte[]> Encrypt(byte[] plainText, string keyId, Func<string, Task<byte[]?>> keyResolver)
    {
        // Validate inputs
        ArgumentNullException.ThrowIfNull(plainText);
        if (string.IsNullOrEmpty(keyId)) throw new ArgumentException("Invalid key ID.");
        ArgumentNullException.ThrowIfNull(keyResolver);
        if (plainText.Length > MaxPlaintextSize) throw new ArgumentException($"Plaintext exceeds maximum size of {MaxPlaintextSize} bytes.");

        // Convert keyId to bytes and validate length
        var keyIdBytes = System.Text.Encoding.ASCII.GetBytes(keyId);
        if (keyIdBytes.Length > MaxKeyIdLength)
            throw new ArgumentException($"Key ID exceeds maximum length of {MaxKeyIdLength} bytes.");

        // Get the key
        var key = await keyResolver(keyId);
        if (key is not { Length: KeySize })
            throw new ArgumentException("Invalid or missing encryption key.");

        using var aesGcm = new AesGcm(key, TagSize);
        
        // Generate random nonce
        var nonce = new byte[NonceSize];
        RandomNumberGenerator.Fill(nonce);

        // Calculate total size: version + keyIdLength + keyId + nonce + tag + ciphertext
        var totalSize = 1 + 1 + keyIdBytes.Length + NonceSize + TagSize + plainText.Length;
        var cipherText = new byte[totalSize];
        var position = 0;

        // Write version
        cipherText[position++] = CurrentVersion;

        // Write keyId length and keyId
        cipherText[position++] = (byte)keyIdBytes.Length;
        keyIdBytes.CopyTo(cipherText, position);
        position += keyIdBytes.Length;

        // Write nonce
        nonce.CopyTo(cipherText, position);
        position += NonceSize;

        // Encrypt the data
        aesGcm.Encrypt(
            nonce,
            plainText,
            cipherText.AsSpan(position + TagSize),  // Ciphertext position
            cipherText.AsSpan(position, TagSize)    // Tag position
        );

        return cipherText;
    }

    /// <summary>
    /// Decrypts data that was encrypted using the Encrypt method.
    /// </summary>
    /// <param name="cipherText">The encrypted data to decrypt.</param>
    /// <param name="keyResolver">Function to retrieve the decryption key based on key ID.</param>
    /// <returns>The decrypted data.</returns>
    public async Task<byte[]> Decrypt(byte[] cipherText, Func<string, Task<byte[]?>> keyResolver)
    {
        // Validate inputs
        ArgumentNullException.ThrowIfNull(cipherText);
        ArgumentNullException.ThrowIfNull(keyResolver);
        if (cipherText.Length < 1 + 1 + NonceSize + TagSize)  // Minimum size check
            throw new ArgumentException("Invalid ciphertext format.");

        var position = 0;

        // Verify version
        var version = cipherText[position++];
        if (version != CurrentVersion)
            throw new ArgumentException($"Unsupported format version: {version}");

        // Extract keyId
        int keyIdLength = cipherText[position++];
        if (keyIdLength > MaxKeyIdLength || cipherText.Length < position + keyIdLength + NonceSize + TagSize)
            throw new ArgumentException("Invalid ciphertext format.");
            
        var keyId = System.Text.Encoding.ASCII.GetString(
            cipherText.AsSpan(position, keyIdLength));
        position += keyIdLength;

        // Get the key
        var key = await keyResolver(keyId);
        if (key is not { Length: KeySize })
            throw new ArgumentException("Invalid or missing encryption key.");

        // Extract nonce
        var nonce = cipherText.AsSpan(position, NonceSize).ToArray();
        position += NonceSize;

        // Extract tag and encrypted data
        ReadOnlySpan<byte> tag = cipherText.AsSpan(position, TagSize);
        position += TagSize;
        ReadOnlySpan<byte> encryptedData = cipherText.AsSpan(position);

        // Decrypt
        var plainText = new byte[encryptedData.Length];
        using var aesGcm = new AesGcm(key, TagSize);
        aesGcm.Decrypt(nonce, encryptedData, tag, plainText);

        return plainText;
    }
}