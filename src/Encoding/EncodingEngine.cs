using System.Text;

namespace VessieFramework.Encoding;

/// <summary>
/// Sistema avançado de codificação e decodificação com múltiplos algoritmos
/// </summary>
public interface IEncoder
{
    string Encode(string input);
    string Decode(string encoded);
    byte[] EncodeBytes(byte[] input);
    byte[] DecodeBytes(byte[] encoded);
    string AlgorithmName { get; }
}

/// <summary>
/// Codificador Base64 padrão
/// </summary>
public class Base64Encoder : IEncoder
{
    public string AlgorithmName => "Base64";

    public string Encode(string input)
        => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));

    public string Decode(string encoded)
        => Encoding.UTF8.GetString(Convert.FromBase64String(encoded));

    public byte[] EncodeBytes(byte[] input)
        => Convert.ToBase64String(input).Select(c => (byte)c).ToArray();

    public byte[] DecodeBytes(byte[] encoded)
    {
        var base64String = Encoding.UTF8.GetString(encoded);
        return Convert.FromBase64String(base64String);
    }
}

/// <summary>
/// Codificador Base64URL (safe para URLs)
/// </summary>
public class Base64UrlEncoder : IEncoder
{
    public string AlgorithmName => "Base64Url";

    public string Encode(string input)
    {
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    public string Decode(string encoded)
    {
        var base64 = encoded.Replace('-', '+').Replace('_', '/');
        while (base64.Length % 4 != 0) base64 += "=";
        return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
    }

    public byte[] EncodeBytes(byte[] input)
        => Encoding.UTF8.GetBytes(Encode(Encoding.UTF8.GetString(input)));

    public byte[] DecodeBytes(byte[] encoded)
        => Decode(Encoding.UTF8.GetString(encoded)).Select(c => (byte)c).ToArray();
}

/// <summary>
/// Codificador Hexadecimal
/// </summary>
public class HexEncoder : IEncoder
{
    public string AlgorithmName => "Hex";

    public string Encode(string input)
        => BitConverter.ToString(Encoding.UTF8.GetBytes(input)).Replace("-", "");

    public string Decode(string encoded)
    {
        var bytes = Enumerable.Range(0, encoded.Length / 2)
            .Select(i => Convert.ToByte(encoded.Substring(i * 2, 2), 16))
            .ToArray();
        return Encoding.UTF8.GetString(bytes);
    }

    public byte[] EncodeBytes(byte[] input)
        => Encoding.UTF8.GetBytes(BitConverter.ToString(input).Replace("-", ""));

    public byte[] DecodeBytes(byte[] encoded)
    {
        var hexString = Encoding.UTF8.GetString(encoded);
        return Enumerable.Range(0, hexString.Length / 2)
            .Select(i => Convert.ToByte(hexString.Substring(i * 2, 2), 16))
            .ToArray();
    }
}

/// <summary>
/// Codificador ROT13 (cifra de César)
/// </summary>
public class Rot13Encoder : IEncoder
{
    public string AlgorithmName => "ROT13";

    public string Encode(string input)
    {
        var result = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            if (c >= 'A' && c <= 'Z')
                result.Append((char)('A' + (c - 'A' + 13) % 26));
            else if (c >= 'a' && c <= 'z')
                result.Append((char)('a' + (c - 'a' + 13) % 26));
            else
                result.Append(c);
        }
        return result.ToString();
    }

    public string Decode(string encoded) => Encode(encoded); // ROT13 é simétrico

    public byte[] EncodeBytes(byte[] input)
        => Encoding.UTF8.GetBytes(Encode(Encoding.UTF8.GetString(input)));

    public byte[] DecodeBytes(byte[] encoded)
        => Encoding.UTF8.GetBytes(Decode(Encoding.UTF8.GetString(encoded)));
}

/// <summary>
/// Codificador XOR com chave
/// </summary>
public class XorEncoder : IEncoder
{
    private readonly byte _key;
    public string AlgorithmName => $"XOR-{_key}";

    public XorEncoder(byte key = 0x5A)
    {
        _key = key;
    }

    public string Encode(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] ^= _key;
        return Convert.ToBase64String(bytes);
    }

    public string Decode(string encoded)
    {
        var bytes = Convert.FromBase64String(encoded);
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] ^= _key;
        return Encoding.UTF8.GetString(bytes);
    }

    public byte[] EncodeBytes(byte[] input)
    {
        var result = new byte[input.Length];
        for (int i = 0; i < input.Length; i++)
            result[i] = (byte)(input[i] ^ _key);
        return result;
    }

    public byte[] DecodeBytes(byte[] encoded) => EncodeBytes(encoded); // XOR é simétrico
}

/// <summary>
/// Codificador Run-Length Encoding (RLE)
/// </summary>
public class RleEncoder : IEncoder
{
    public string AlgorithmName => "RLE";

    public string Encode(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        
        var result = new StringBuilder();
        int count = 1;
        
        for (int i = 1; i < input.Length; i++)
        {
            if (input[i] == input[i - 1])
                count++;
            else
            {
                result.Append(count).Append(input[i - 1]);
                count = 1;
            }
        }
        result.Append(count).Append(input[^1]);
        return result.ToString();
    }

    public string Decode(string encoded)
    {
        if (string.IsNullOrEmpty(encoded)) return string.Empty;
        
        var result = new StringBuilder();
        int i = 0;
        
        while (i < encoded.Length)
        {
            int count = 0;
            while (i < encoded.Length && char.IsDigit(encoded[i]))
                count = count * 10 + (encoded[i++] - '0');
            
            if (i < encoded.Length)
                result.Append(new string(encoded[i++], count));
        }
        return result.ToString();
    }

    public byte[] EncodeBytes(byte[] input)
        => Encoding.UTF8.GetBytes(Encode(Encoding.UTF8.GetString(input)));

    public byte[] DecodeBytes(byte[] encoded)
        => Encoding.UTF8.GetBytes(Decode(Encoding.UTF8.GetString(encoded)));
}

/// <summary>
/// Fábrica de codificadores
/// </summary>
public static class EncoderFactory
{
    private static readonly Dictionary<string, IEncoder> _encoders = new()
    {
        ["base64"] = new Base64Encoder(),
        ["base64url"] = new Base64UrlEncoder(),
        ["hex"] = new HexEncoder(),
        ["rot13"] = new Rot13Encoder(),
        ["xor"] = new XorEncoder(),
        ["rle"] = new RleEncoder()
    };

    public static IEncoder GetEncoder(string algorithm)
    {
        if (_encoders.TryGetValue(algorithm.ToLower(), out var encoder))
            return encoder;
        throw new ArgumentException($"Algoritmo '{algorithm}' não suportado");
    }

    public static IEnumerable<string> GetSupportedAlgorithms()
        => _encoders.Keys;

    public static string Encode(string input, string algorithm)
        => GetEncoder(algorithm).Encode(input);

    public static string Decode(string encoded, string algorithm)
        => GetEncoder(algorithm).Decode(encoded);
}
