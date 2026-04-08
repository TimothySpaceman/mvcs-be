using Lib.Modules.Storages.Entities;
using Lib.Modules.Storages.Entities.Schema;

namespace Lib.Modules.Storages.DTOs;

public record StorageTypeDto(
    Guid Id,
    string Key,
    string Label,
    string Description,
    StorageConfigSchema ConfigSchema
)
{
    public static StorageTypeDto FromStorageType(StorageType storageType) => new(
        storageType.Id,
        storageType.Key,
        storageType.Label,
        storageType.Description,
        storageType.ConfigSchema
    );
}
