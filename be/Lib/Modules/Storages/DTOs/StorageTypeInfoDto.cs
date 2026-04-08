using Lib.Modules.Storages.Entities;

namespace Lib.Modules.Storages.DTOs;

public record StorageTypeInfoDto(
    Guid Id,
    string Key,
    string Label,
    string Description
)
{
    public static StorageTypeInfoDto FromStorageType(StorageType storageType)
    {
        return new StorageTypeInfoDto(
            storageType.Id,
            storageType.Key,
            storageType.Label,
            storageType.Description
        );
    }
}