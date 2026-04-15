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
    StorageAccessType AccessType,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
)
{
    public static StorageDto FromStorage(Storage storage, Guid requestingUserId)
    {
        var accessType = storage.IsPublic
            ? StorageAccessType.ReadWrite
            : storage.AccessEntries.First(a => a.UserId == requestingUserId).AccessType;

        return new StorageDto(
            storage.Id,
            storage.Name,
            storage.StorageTypeId,
            storage.StorageType.Key,
            storage.StorageType.Label,
            storage.IsPublic,
            storage.IsDefault,
            accessType,
            storage.CreatedAt,
            storage.UpdatedAt
        );
    }
}