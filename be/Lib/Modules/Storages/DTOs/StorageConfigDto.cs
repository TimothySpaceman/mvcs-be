using Lib.Modules.Storages.Entities;

namespace Lib.Modules.Storages.DTOs;

public record StorageConfigDto(StorageTypeDto Type, Dictionary<string, object?> Config)
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
 
        return new StorageConfigDto(StorageTypeDto.FromStorageType(storage.StorageType), raw);
    }
}