using Lib.Modules.Storages.Entities;

namespace Lib.Modules.Storages.DTOs;

public record StorageConfigDto(Guid Id, Dictionary<string, object?> Config)
{
    public static StorageConfigDto FromStorage(Storage storage)
    {
        var raw = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object?>>(storage.Config)
                  ?? [];
 
        var passwordKeys = storage.StorageType.ConfigSchema.Fields
            .Where(f => f.Type == Entities.Schema.SchemaFieldType.Password)
            .Select(f => f.Key)
            .ToHashSet();

        foreach (var key in passwordKeys)
        {
            if (raw.ContainsKey(key)) raw[key] = null;
        }
 
        return new StorageConfigDto(storage.Id, raw);
    }
}