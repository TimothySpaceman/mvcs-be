using Core.FileChanges;

namespace Lib.Modules.Vcs.DTOs;

public record FileChangeDto(
    FileSnapshotDto? Before,
    FileSnapshotDto? After
)
{
    public static FileChangeDto FromDomain(FileChange domain)
    {
        return new FileChangeDto(
            domain.Before is null ? null : FileSnapshotDto.FromDomain(domain.Before),
            domain.After is null ? null : FileSnapshotDto.FromDomain(domain.After)
        );
    }
}