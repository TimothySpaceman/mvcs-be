using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Storage;

namespace Lib.Modules.Vcs.Converters;

public class HashIdJsonConverter : JsonConverter<HashId>
{
    public override HashId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var hex = reader.GetString();

        if (string.IsNullOrWhiteSpace(hex)) throw new JsonException("HashId value is null or empty");

        try
        {
            var bytes = Convert.FromHexString(hex);
            return new HashId(bytes);
        }
        catch (FormatException ex)
        {
            throw new JsonException("Invalid hex string", ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, HashId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Convert.ToHexString(value.Bytes.Span));
    }
}