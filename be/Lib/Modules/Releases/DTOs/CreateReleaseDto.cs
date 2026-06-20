namespace Lib.Modules.Releases.DTOs;

public record CreateReleaseDto(
    string Title,
    List<CreateReleaseFileDto> Files
);