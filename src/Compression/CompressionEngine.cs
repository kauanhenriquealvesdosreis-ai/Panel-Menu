using System.IO.Compression;
using System.Text;

namespace VessieFramework.Compression;

/// <summary>
/// Sistema avançado de compressão com múltiplos algoritmos
/// </summary>
public interface ICompressor
{
    byte[] Compress(byte[] data);
    byte[] Decompress(byte[] compressedData);
    string CompressString(string input);
    string DecompressString(string compressed);
    string AlgorithmName { get; }
    int CompressionLevel { get; }
}

/// <summary>
/// Compressão GZip
/// </summary>
public class GZipCompressor : ICompressor
{
    public string AlgorithmName => "GZip";
    public int CompressionLevel => (int)CompressionLevel.Optimal;

    public byte[] Compress(byte[] data)
    {
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionMode.Compress, true))
        {
            gzip.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

    public byte[] Decompress(byte[] compressedData)
    {
        using var input = new MemoryStream(compressedData);
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(input, CompressionMode.Decompress))
        {
            gzip.CopyTo(output);
        }
        return output.ToArray();
    }

    public string CompressString(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var compressed = Compress(bytes);
        return Convert.ToBase64String(compressed);
    }

    public string DecompressString(string compressed)
    {
        var bytes = Convert.FromBase64String(compressed);
        var decompressed = Decompress(bytes);
        return Encoding.UTF8.GetString(decompressed);
    }
}

/// <summary>
/// Compressão Deflate
/// </summary>
public class DeflateCompressor : ICompressor
{
    public string AlgorithmName => "Deflate";
    public int CompressionLevel => (int)CompressionLevel.Fastest;

    public byte[] Compress(byte[] data)
    {
        using var output = new MemoryStream();
        using (var deflate = new DeflateStream(output, CompressionMode.Compress, true))
        {
            deflate.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

    public byte[] Decompress(byte[] compressedData)
    {
        using var input = new MemoryStream(compressedData);
        using var output = new MemoryStream();
        using (var deflate = new DeflateStream(input, CompressionMode.Decompress))
        {
            deflate.CopyTo(output);
        }
        return output.ToArray();
    }

    public string CompressString(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var compressed = Compress(bytes);
        return Convert.ToBase64String(compressed);
    }

    public string DecompressString(string compressed)
    {
        var bytes = Convert.FromBase64String(compressed);
        var decompressed = Decompress(bytes);
        return Encoding.UTF8.GetString(decompressed);
    }
}

/// <summary>
/// Compressão Brotli (melhor taxa)
/// </summary>
public class BrotliCompressor : ICompressor
{
    public string AlgorithmName => "Brotli";
    public int CompressionLevel => (int)CompressionLevel.SmallestSize;

    public byte[] Compress(byte[] data)
    {
        using var output = new MemoryStream();
        using (var brotli = new BrotliStream(output, CompressionMode.Compress, true))
        {
            brotli.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

    public byte[] Decompress(byte[] compressedData)
    {
        using var input = new MemoryStream(compressedData);
        using var output = new MemoryStream();
        using (var brotli = new BrotliStream(input, CompressionMode.Decompress))
        {
            brotli.CopyTo(output);
        }
        return output.ToArray();
    }

    public string CompressString(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var compressed = Compress(bytes);
        return Convert.ToBase64String(compressed);
    }

    public string DecompressString(string compressed)
    {
        var bytes = Convert.FromBase64String(compressed);
        var decompressed = Decompress(bytes);
        return Encoding.UTF8.GetString(decompressed);
    }
}

/// <summary>
/// Compressão LZ4 (rápida) - Simulado com algoritmo próprio
/// </summary>
public class Lz4Compressor : ICompressor
{
    public string AlgorithmName => "LZ4-Like";
    public int CompressionLevel => 1;

    public byte[] Compress(byte[] data)
    {
        // Implementação simplificada de compressão LZ-like
        if (data.Length == 0) return Array.Empty<byte>();
        
        using var output = new MemoryStream();
        using var writer = new BinaryWriter(output);
        
        writer.Write(data.Length); // Tamanho original
        
        int i = 0;
        while (i < data.Length)
        {
            // Procura por repetições
            int matchLen = 0;
            int matchPos = 0;
            
            for (int j = Math.Max(0, i - 4096); j < i && matchLen < 18; j++)
            {
                int len = 0;
                while (i + len < data.Length && data[j + len] == data[i + len] && len < 18)
                    len++;
                
                if (len > matchLen && len >= 3)
                {
                    matchLen = len;
                    matchPos = j;
                }
            }
            
            if (matchLen >= 3)
            {
                // Token de cópia: bit alto = 1, seguido por offset e tamanho
                writer.Write((byte)(0x80 | (matchLen - 3)));
                writer.Write((ushort)(i - matchPos));
                i += matchLen;
            }
            else
            {
                // Token literal: bit alto = 0
                writer.Write(data[i]);
                i++;
            }
        }
        
        return output.ToArray();
    }

    public byte[] Decompress(byte[] compressedData)
    {
        using var input = new MemoryStream(compressedData);
        using var reader = new BinaryReader(input);
        
        var originalLength = reader.ReadInt32();
        using var output = new MemoryStream(originalLength);
        
        while (output.Position < originalLength && input.Position < input.Length)
        {
            var token = reader.ReadByte();
            
            if ((token & 0x80) != 0)
            {
                // Cópias
                var matchLen = (token & 0x7F) + 3;
                var offset = reader.ReadUInt16();
                var startPos = (int)output.Position - offset;
                
                for (int i = 0; i < matchLen; i++)
                {
                    output.Position = startPos + i;
                    var b = output.ReadByte();
                    output.Position = startPos + i + offset;
                    output.WriteByte(b);
                }
            }
            else
            {
                // Literal
                output.WriteByte(token);
            }
        }
        
        return output.ToArray();
    }

    public string CompressString(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var compressed = Compress(bytes);
        return Convert.ToBase64String(compressed);
    }

    public string DecompressString(string compressed)
    {
        var bytes = Convert.FromBase64String(compressed);
        var decompressed = Decompress(bytes);
        return Encoding.UTF8.GetString(decompressed);
    }
}

/// <summary>
/// Compressão Run-Length (RLE)
/// </summary>
public class RleCompressor : ICompressor
{
    public string AlgorithmName => "RLE";
    public int CompressionLevel => 0;

    public byte[] Compress(byte[] data)
    {
        if (data.Length == 0) return Array.Empty<byte>();
        
        using var output = new MemoryStream();
        byte current = data[0];
        int count = 1;
        
        for (int i = 1; i < data.Length; i++)
        {
            if (data[i] == current && count < 255)
            {
                count++;
            }
            else
            {
                output.WriteByte(current);
                output.WriteByte((byte)count);
                current = data[i];
                count = 1;
            }
        }
        
        output.WriteByte(current);
        output.WriteByte((byte)count);
        
        return output.ToArray();
    }

    public byte[] Decompress(byte[] compressedData)
    {
        using var output = new MemoryStream();
        
        for (int i = 0; i < compressedData.Length - 1; i += 2)
        {
            byte value = compressedData[i];
            byte count = compressedData[i + 1];
            
            for (int j = 0; j < count; j++)
            {
                output.WriteByte(value);
            }
        }
        
        return output.ToArray();
    }

    public string CompressString(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var compressed = Compress(bytes);
        return Convert.ToBase64String(compressed);
    }

    public string DecompressString(string compressed)
    {
        var bytes = Convert.FromBase64String(compressed);
        var decompressed = Decompress(bytes);
        return Encoding.UTF8.GetString(decompressed);
    }
}

/// <summary>
/// Fábrica de compressores
/// </summary>
public static class CompressorFactory
{
    private static readonly Dictionary<string, ICompressor> _compressors = new()
    {
        ["gzip"] = new GZipCompressor(),
        ["deflate"] = new DeflateCompressor(),
        ["brotli"] = new BrotliCompressor(),
        ["lz4"] = new Lz4Compressor(),
        ["rle"] = new RleCompressor()
    };

    public static ICompressor GetCompressor(string algorithm)
    {
        if (_compressors.TryGetValue(algorithm.ToLower(), out var compressor))
            return compressor;
        throw new ArgumentException($"Algoritmo '{algorithm}' não suportado");
    }

    public static IEnumerable<string> GetSupportedAlgorithms()
        => _compressors.Keys;

    public static byte[] Compress(byte[] data, string algorithm = "gzip")
        => GetCompressor(algorithm).Compress(data);

    public static byte[] Decompress(byte[] data, string algorithm = "gzip")
        => GetCompressor(algorithm).Decompress(data);

    public static string CompressString(string input, string algorithm = "gzip")
        => GetCompressor(algorithm).CompressString(input);

    public static string DecompressString(string compressed, string algorithm = "gzip")
        => GetCompressor(algorithm).DecompressString(compressed);

    /// <summary>
    /// Calcula a taxa de compressão
    /// </summary>
    public static double GetCompressionRatio(byte[] original, byte[] compressed)
        => (1.0 - (double)compressed.Length / original.Length) * 100;

    /// <summary>
    /// Compressão em cascata (múltiplos algoritmos)
    /// </summary>
    public static byte[] CascadeCompress(byte[] data, params string[] algorithms)
    {
        var result = data;
        foreach (var algo in algorithms)
        {
            result = GetCompressor(algo).Compress(result);
        }
        return result;
    }

    /// <summary>
    /// Descompressão em cascata (ordem inversa)
    /// </summary>
    public static byte[] CascadeDecompress(byte[] data, params string[] algorithms)
    {
        var result = data;
        for (int i = algorithms.Length - 1; i >= 0; i--)
        {
            result = GetCompressor(algorithms[i]).Decompress(result);
        }
        return result;
    }
}
