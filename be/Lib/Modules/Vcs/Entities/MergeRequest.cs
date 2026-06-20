using Core.Storage;

namespace Lib.Modules.Vcs.Entities;

public class MergeRequest
{
    public Guid Id { get; private set; }
    public Guid AuthorId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = null!;
    public string TargetRefName { get; private set; } = null!;
    public string SourceRefName { get; private set; } = null!;
    public HashId MergeCommitId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private MergeRequest()
    {
    }

    public static MergeRequest Create(
        Guid authorId,
        Guid projectId,
        string title,
        string targetRefName,
        string sourceRefName,
        HashId mergeCommitId
    )
    {
        return new MergeRequest
        {
            Id = Guid.NewGuid(),
            AuthorId = authorId,
            ProjectId = projectId,
            Title = title,
            TargetRefName = targetRefName,
            SourceRefName = sourceRefName,
            MergeCommitId = mergeCommitId,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
}