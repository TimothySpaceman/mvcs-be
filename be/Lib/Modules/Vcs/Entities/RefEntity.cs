using Core.Storage;

namespace Lib.Modules.Vcs.Entities;

public class RefEntity
{
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; } = null!;
    public HashId? CommitId { get; private set; }

    private RefEntity()
    {
    }

    public static RefEntity Create(Guid projectId, string name, HashId? commitId = null)
    {
        return new RefEntity
        {
            ProjectId = projectId,
            Name = name,
            CommitId = commitId
        };
    }

    public void SetCommitId(HashId? commitId)
    {
        CommitId = commitId;
    }
}