using System.Collections.Immutable;
using Core.Snapshots;

namespace Lib.Modules.Vcs.DTOs;

public record SnapshotDto(
    ImmutableDictionary<string, FileSnapshotDto> Files
)
{
    public static SnapshotDto FromDomain(Snapshot domain)
    {
        return new SnapshotDto(domain.Files.ToImmutableDictionary(
            x => x.Key,
            x => FileSnapshotDto.FromDomain(x.Value)
        ));
    }
}