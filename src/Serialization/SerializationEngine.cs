using System.Text;
using System.Text.Json;

namespace VessieFramework.Serialization;

/// <summary>
/// Sistema avançado de serialização com múltiplos formatos
/// </summary>
public interface ISerializer
{
    string Serialize<T>(T obj);
    T? Deserialize<T>(string data);
    byte[] SerializeBytes<T>(T obj);
    T? DeserializeBytes<T>(byte[] data);
    string FormatName { get; }
}

/// <summary>
/// Serializador JSON (System.Text.Json)
/// </summary>
public class JsonSerializer : ISerializer
{
    public string FormatName => "JSON";
    private readonly JsonSerializerOptions _options;

    public JsonSerializer()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public string Serialize<T>(T obj)
        => System.Text.Json.JsonSerializer.Serialize(obj, _options);

    public T? Deserialize<T>(string data)
        => System.Text.Json.JsonSerializer.Deserialize<T>(data, _options);

    public byte[] SerializeBytes<T>(T obj)
        => Encoding.UTF8.GetBytes(Serialize(obj));

    public T? DeserializeBytes<T>(byte[] data)
        => Deserialize<T>(Encoding.UTF8.GetString(data));
}

/// <summary>
/// Serializador XML
/// </summary>
public class XmlSerializer : ISerializer
{
    public string FormatName => "XML";

    public string Serialize<T>(T obj)
    {
        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        using var writer = new StringWriter();
        xmlSerializer.Serialize(writer, obj);
        return writer.ToString();
    }

    public T? Deserialize<T>(string data)
    {
        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        using var reader = new StringReader(data);
        return (T?)xmlSerializer.Deserialize(reader);
    }

    public byte[] SerializeBytes<T>(T obj)
        => Encoding.UTF8.GetBytes(Serialize<T>(obj));

    public T? DeserializeBytes<T>(byte[] data)
        => Deserialize<T>(Encoding.UTF8.GetString(data));
}

/// <summary>
/// Serializador Binary (formato compacto)
/// </summary>
public class BinarySerializer : ISerializer
{
    public string FormatName => "Binary";

    public string Serialize<T>(T obj)
    {
        var bytes = SerializeBytes(obj);
        return Convert.ToBase64String(bytes);
    }

    public T? Deserialize<T>(string data)
    {
        var bytes = Convert.FromBase64String(data);
        return DeserializeBytes<T>(bytes);
    }

    public byte[] SerializeBytes<T>(T obj)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        
        // Escreve o tipo
        writer.Write(typeof(T).FullName ?? "");
        
        // Serializa propriedades simples
        if (obj is string s)
        {
            writer.Write(s);
        }
        else if (obj is int i)
        {
            writer.Write(i);
        }
        else if (obj is long l)
        {
            writer.Write(l);
        }
        else if (obj is double d)
        {
            writer.Write(d);
        }
        else if (obj is bool b)
        {
            writer.Write(b);
        }
        else if (obj is byte[] arr)
        {
            writer.Write(arr.Length);
            writer.Write(arr);
        }
        else if (obj != null)
        {
            // Fallback para JSON para objetos complexos
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            writer.Write(json);
        }
        
        return ms.ToArray();
    }

    public T? DeserializeBytes<T>(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);
        
        var typeName = reader.ReadString();
        
        if (typeof(T) == typeof(string))
        {
            return (T?)(object)reader.ReadString();
        }
        else if (typeof(T) == typeof(int))
        {
            return (T?)(object)reader.ReadInt32();
        }
        else if (typeof(T) == typeof(long))
        {
            return (T?)(object)reader.ReadInt64();
        }
        else if (typeof(T) == typeof(double))
        {
            return (T?)(object)reader.ReadDouble();
        }
        else if (typeof(T) == typeof(bool))
        {
            return (T?)(object)reader.ReadBoolean();
        }
        else if (typeof(T) == typeof(byte[]))
        {
            var length = reader.ReadInt32();
            var bytes = reader.ReadBytes(length);
            return (T?)(object)bytes;
        }
        else
        {
            // Fallback para JSON para objetos complexos
            var json = reader.ReadString();
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
    }
}

/// <summary>
/// Serializador CSV simples
/// </summary>
public class CsvSerializer : ISerializer
{
    public string FormatName => "CSV";

    public string Serialize<T>(T obj)
    {
        if (obj is IEnumerable enumerable && obj is not string)
        {
            var sb = new StringBuilder();
            var items = enumerable.Cast<object>().ToList();
            if (items.Count == 0) return string.Empty;
            
            // Cabeçalho
            var properties = typeof(T).GetGenericArguments().Length > 0 
                ? items[0].GetType().GetProperties()
                : typeof(object).GetProperties();
            
            sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));
            
            // Dados
            foreach (var item in items)
            {
                var values = properties.Select(p => 
                {
                    var value = p.GetValue(item)?.ToString() ?? "";
                    return value.Contains(",") ? $"\"{value}\"" : value;
                });
                sb.AppendLine(string.Join(",", values));
            }
            
            return sb.ToString();
        }
        
        return SerializeSingle(obj);
    }

    private string SerializeSingle<T>(T obj)
    {
        var sb = new StringBuilder();
        var properties = typeof(T).GetProperties();
        
        // Cabeçalho
        sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));
        
        // Dados
        var values = properties.Select(p => 
        {
            var value = p.GetValue(obj)?.ToString() ?? "";
            return value.Contains(",") ? $"\"{value}\"" : value;
        });
        sb.AppendLine(string.Join(",", values));
        
        return sb.ToString();
    }

    public T? Deserialize<T>(string data)
    {
        // Implementação simplificada
        throw new NotImplementedException("Deserialização CSV requer tipo específico");
    }

    public byte[] SerializeBytes<T>(T obj)
        => Encoding.UTF8.GetBytes(Serialize(obj));

    public T? DeserializeBytes<T>(byte[] data)
        => Deserialize<T>(Encoding.UTF8.GetString(data));
}

/// <summary>
/// Fábrica de serializadores
/// </summary>
public static class SerializerFactory
{
    private static readonly Dictionary<string, ISerializer> _serializers = new()
    {
        ["json"] = new JsonSerializer(),
        ["xml"] = new XmlSerializer(),
        ["binary"] = new BinarySerializer(),
        ["csv"] = new CsvSerializer()
    };

    public static ISerializer GetSerializer(string format)
    {
        if (_serializers.TryGetValue(format.ToLower(), out var serializer))
            return serializer;
        throw new ArgumentException($"Formato '{format}' não suportado");
    }

    public static IEnumerable<string> GetSupportedFormats()
        => _serializers.Keys;

    public static string Serialize<T>(T obj, string format = "json")
        => GetSerializer(format).Serialize(obj);

    public static T? Deserialize<T>(string data, string format = "json")
        => GetSerializer(format).Deserialize<T>(data);

    public static byte[] SerializeBytes<T>(T obj, string format = "json")
        => GetSerializer(format).SerializeBytes(obj);

    public static T? DeserializeBytes<T>(byte[] data, string format = "json")
        => GetSerializer(format).DeserializeBytes<T>(data);
}

/// <summary>
/// Extensões para serialização rápida
/// </summary>
public static class SerializationExtensions
{
    public static string ToJson<T>(this T obj)
        => SerializerFactory.Serialize(obj, "json");

    public static T? FromJson<T>(this string json)
        => SerializerFactory.Deserialize<T>(json, "json");

    public static string ToXml<T>(this T obj)
        => SerializerFactory.Serialize(obj, "xml");

    public static T? FromXml<T>(this string xml)
        => SerializerFactory.Deserialize<T>(xml, "xml");

    public static string ToBase64<T>(this T obj)
        => SerializerFactory.GetSerializer("binary").Serialize(obj);

    public static T? FromBase64<T>(this string base64)
        => SerializerFactory.GetSerializer("binary").Deserialize<T>(base64);
}
