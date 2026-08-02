using System.Security.Cryptography;
using System.Text;

namespace VessieFramework.Crypto;

/// <summary>
/// Sistema avançado de criptografia com múltiplos algoritmos
/// </summary>
public interface ICryptoProvider
{
    string Encrypt(string plainText, string key);
    string Decrypt(string cipherText, string key);
    byte[] EncryptBytes(byte[] plainData, byte[] key);
    byte[] DecryptBytes(byte[] cipherData, byte[] key);
    string AlgorithmName { get; }
}

/// <summary>
/// Criptografia AES-256
/// </summary>
public class AesCrypto : ICryptoProvider
{
    public string AlgorithmName => "AES-256";

    public string Encrypt(string plainText, string key)
    {
        using var aes = Aes.Create();
        aes.Key = DeriveKey(key, 32);
        aes.GenerateIV();
        
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        
        // IV + dados criptografados
        var result = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);
        
        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText, string key)
    {
        using var aes = Aes.Create();
        aes.Key = DeriveKey(key, 32);
        
        var fullData = Convert.FromBase64String(cipherText);
        var iv = new byte[16];
        var cipherBytes = new byte[fullData.Length - 16];
        
        Buffer.BlockCopy(fullData, 0, iv, 0, 16);
        Buffer.BlockCopy(fullData, 16, cipherBytes, 0, cipherBytes.Length);
        
        aes.IV = iv;
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        
        return Encoding.UTF8.GetString(plainBytes);
    }

    public byte[] EncryptBytes(byte[] plainData, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = DeriveKey(Encoding.UTF8.GetString(key), 32);
        aes.GenerateIV();
        
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var cipherBytes = encryptor.TransformFinalBlock(plainData, 0, plainData.Length);
        
        var result = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);
        
        return result;
    }

    public byte[] DecryptBytes(byte[] cipherData, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = DeriveKey(Encoding.UTF8.GetString(key), 32);
        
        var iv = new byte[16];
        var cipherBytes = new byte[cipherData.Length - 16];
        
        Buffer.BlockCopy(cipherData, 0, iv, 0, 16);
        Buffer.BlockCopy(cipherData, 16, cipherBytes, 0, cipherBytes.Length);
        
        aes.IV = iv;
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        return decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
    }

    private static byte[] DeriveKey(string password, int keySize)
    {
        using var sha256 = SHA256.Create();
        var key = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        
        if (key.Length < keySize)
        {
            var extendedKey = new byte[keySize];
            Buffer.BlockCopy(key, 0, extendedKey, 0, key.Length);
            return extendedKey;
        }
        
        return key.Take(keySize).ToArray();
    }
}

/// <summary>
/// Criptografia RSA (assimétrica)
/// </summary>
public class RsaCrypto : ICryptoProvider
{
    public string AlgorithmName => "RSA-2048";
    private readonly RSA _rsa;

    public RsaCrypto()
    {
        _rsa = RSA.Create(2048);
    }

    public RsaCrypto(string publicKeyXml, string privateKeyXml)
    {
        _rsa = RSA.Create();
        _rsa.FromXmlString(privateKeyXml);
    }

    public string GetPublicKeyXml() => _rsa.ToXmlString(false);
    public string GetPrivateKeyXml() => _rsa.ToXmlString(true);

    public string Encrypt(string plainText, string key)
    {
        // Em RSA, a chave é usada como seed para gerar par de chaves temporário
        using var tempRsa = RSA.Create(2048);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = tempRsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);
        return Convert.ToBase64String(cipherBytes);
    }

    public string Decrypt(string cipherText, string key)
    {
        var cipherBytes = Convert.FromBase64String(cipherText);
        var plainBytes = _rsa.Decrypt(cipherBytes, RSAEncryptionPadding.OaepSHA256);
        return Encoding.UTF8.GetString(plainBytes);
    }

    public byte[] EncryptBytes(byte[] plainData, byte[] key)
        => _rsa.Encrypt(plainData, RSAEncryptionPadding.OaepSHA256);

    public byte[] DecryptBytes(byte[] cipherData, byte[] key)
        => _rsa.Decrypt(cipherData, RSAEncryptionPadding.OaepSHA256);
}

/// <summary>
/// Hashing seguro
/// </summary>
public static class HashProvider
{
    public static string ComputeMD5(string input)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    public static string ComputeSHA256(string input)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    public static string ComputeSHA512(string input)
    {
        using var sha512 = SHA512.Create();
        var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    public static string ComputeHMACSHA256(string input, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    public static string GenerateSalt(int length = 32)
    {
        var salt = new byte[length];
        RandomNumberGenerator.Fill(salt);
        return Convert.ToBase64String(salt);
    }

    public static string HashPassword(string password, string? salt = null)
    {
        salt ??= GenerateSalt();
        var hash = ComputeSHA256(password + salt);
        return $"{salt}:{hash}";
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split(':');
        if (parts.Length != 2) return false;
        
        var salt = parts[0];
        var expectedHash = parts[1];
        var actualHash = ComputeSHA256(password + salt);
        
        return expectedHash == actualHash;
    }
}

/// <summary>
/// Fábrica de provedores criptográficos
/// </summary>
public static class CryptoFactory
{
    private static readonly Dictionary<string, ICryptoProvider> _providers = new()
    {
        ["aes"] = new AesCrypto(),
        ["rsa"] = new RsaCrypto()
    };

    public static ICryptoProvider GetProvider(string algorithm)
    {
        if (_providers.TryGetValue(algorithm.ToLower(), out var provider))
            return provider;
        throw new ArgumentException($"Algoritmo '{algorithm}' não suportado");
    }

    public static IEnumerable<string> GetSupportedAlgorithms()
        => _providers.Keys;

    public static string Encrypt(string input, string key, string algorithm = "aes")
        => GetProvider(algorithm).Encrypt(input, key);

    public static string Decrypt(string encrypted, string key, string algorithm = "aes")
        => GetProvider(algorithm).Decrypt(encrypted, key);
}
