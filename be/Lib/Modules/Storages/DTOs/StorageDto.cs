using Lib.Modules.Storages.Entities;

namespace Lib.Modules.Storages.DTOs;

public record StorageDto(
    Guid Id,
    string Name,
    Guid StorageTypeId,
    string TypeKey,
    string TypeLabel,
    bool IsDefault,
    bool IsPublic,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
)
{
    public static StorageDto FromStorage(Storage storage)
    {
        return new StorageDto(
            storage.Id,
            storage.Name,
            storage.StorageTypeId,
            storage.StorageType.Key,
            storage.StorageType.Label,
            storage.IsPublic,
            storage.IsDefault,
            storage.CreatedAt,
            storage.UpdatedAt
        );
    }
}