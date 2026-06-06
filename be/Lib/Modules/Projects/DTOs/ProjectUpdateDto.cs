namespace Lib.Modules.Projects.DTOs;

public record ProjectUpdateDto(
    string? Title,
    string? Description,
    bool? IsPublic,
    string? DefaultRefName
);