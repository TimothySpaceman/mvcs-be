using Lib.Modules.Storages.Entities;

namespace Lib.Modules.Storages.DTOs;

public record StorageDto(
    Guid Id,
    string Name,
    StorageTypeInfoDto StorageType,
    bool IsDefault,
    bool IsPublic,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    StorageAccessLevel AccessLevel
)
{
    public static StorageDto FromStorage(Storage storage, Guid? userId = null)
    {
        var accessLevel = StorageAccessLevel.Public;
        if (userId is not null)
        {
            var entry = storage.AccessEntries.FirstOrDefault(a => a.UserId == userId);
            if (entry is not null) accessLevel = entry.IsOwner ? StorageAccessLevel.Owner : StorageAccessLevel.Write;
        }
        
        return new StorageDto(
            storage.Id,
            storage.Name,
            StorageTypeInfoDto.FromStorageType(storage.StorageType),
            storage.IsPublic,
            storage.IsDefault,
            storage.CreatedAt,
            storage.UpdatedAt,
            accessLevel
        );
    }
}