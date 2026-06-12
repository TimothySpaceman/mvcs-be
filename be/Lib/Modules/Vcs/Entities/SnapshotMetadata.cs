using Core.Storage;

namespace Lib.Modules.Vcs.Entities;

public class SnapshotMetadata
{
    public HashId CommitId { get; private set; }
    public Guid ProjectId { get; private set; }
    public Dictionary<string, string[]> Data { get; private set; } = null!;
    public DateTimeOffset SubmittedAt { get; private set; }

    private SnapshotMetadata()
    {
    }

    public static SnapshotMetadata Create(
        HashId commitId,
        Guid projectId,
        Dictionary<string, string[]> data,
        DateTimeOffset submittedAt
    )
    {
        return new SnapshotMetadata
        {
            CommitId = commitId,
            ProjectId = projectId,
            Data = data,
            SubmittedAt = submittedAt
        };
    }
}