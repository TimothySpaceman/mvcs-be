using Lib.Modules.Storages.Entities;

namespace Lib.Modules.Storages.DTOs;

public record StorageDto(
    Guid Id,
    string Name,
    StorageTypeInfoDto StorageType,
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
            StorageTypeInfoDto.FromStorageType(storage.StorageType),
            storage.IsPublic,
            storage.IsDefault,
            storage.CreatedAt,
            storage.UpdatedAt
        );
    }
}