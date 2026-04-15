using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lib.Modules.Storages.Entities.Schema;

public class SchemaFieldJsonConverter : JsonConverter<SchemaField>
{
    public override SchemaField? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
 
        if (!root.TryGetProperty("$type", out var typeProp))
            throw new JsonException("Missing '$type' discriminator on SchemaField");
 
        var raw = root.GetRawText();
 
        return typeProp.GetString() switch
        {
            "Text"     => JsonSerializer.Deserialize<TextSchemaField>(raw, options),
            "Number"   => JsonSerializer.Deserialize<NumberSchemaField>(raw, options),
            "Boolean"  => JsonSerializer.Deserialize<BooleanSchemaField>(raw, options),
            "Select"   => JsonSerializer.Deserialize<SelectSchemaField>(raw, options),
            "Password" => JsonSerializer.Deserialize<PasswordSchemaField>(raw, options),
            var t      => throw new JsonException($"Unknown SchemaField type discriminator: '{t}'")
        };
    }
 
    public override void Write(Utf8JsonWriter writer, SchemaField value, JsonSerializerOptions options)
    {
        var type = value.GetType();
        JsonSerializer.Serialize(writer, value, type, options);
    }
}
