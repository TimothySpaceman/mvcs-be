namespace Lib.Modules.Projects.DTOs;

public record ProjectCreateDto(
    Guid StorageId,
    string Title,
    string Description,
    bool IsPublic
);