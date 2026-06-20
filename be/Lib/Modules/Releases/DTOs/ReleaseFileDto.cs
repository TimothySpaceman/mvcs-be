using Lib.Modules.Releases.Entities;

namespace Lib.Modules.Releases.DTOs;

public record ReleaseFileDto(
    Guid Id,
    Guid ReleaseId,
    string FileName,
    string BlobId,
    DateTimeOffset CreatedAt
)
{
    public static ReleaseFileDto FromEntity(ReleaseFile entity) => new(
        entity.Id,
        entity.ReleaseId,
        entity.FileName,
        entity.BlobId,
        entity.CreatedAt
    );
}