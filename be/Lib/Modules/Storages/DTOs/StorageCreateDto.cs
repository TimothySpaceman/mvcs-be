namespace Lib.Modules.Storages.DTOs;

public record StorageCreateDto(
    string Name,
    Guid StorageTypeId,
    string Config
);