using Core.Storage;
using Lib.Modules.Vcs.Entities;

namespace Lib.Modules.Vcs.DTOs;

public record SnapshotMetadataDto(
    HashId CommitId,
    Dictionary<string, string[]> Data,
    DateTimeOffset SubmittedAt
)
{
    public static SnapshotMetadataDto FromEntity(SnapshotMetadata entity)
    {
        return new SnapshotMetadataDto(
            entity.CommitId,
            entity.Data,
            entity.SubmittedAt
        );
    }
}