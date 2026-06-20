using Lib.Modules.Releases.Entities;

namespace Lib.Modules.Releases.DTOs;

public record ReleaseDto(
    Guid Id,
    Guid ProjectId,
    Guid? AuthorId,
    string Title,
    DateTimeOffset CreatedAt,
    List<ReleaseFileDto> Files
)
{
    public static ReleaseDto FromEntity(Release entity) => new(
        entity.Id,
        entity.ProjectId,
        entity.AuthorId,
        entity.Title,
        entity.CreatedAt,
        entity.Files.Select(ReleaseFileDto.FromEntity).ToList()
    );
}