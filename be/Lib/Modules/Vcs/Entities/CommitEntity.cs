using System.Collections.Immutable;
using Core.FileChanges;
using Core.Storage;

namespace Lib.Modules.Vcs.Entities;

public class CommitEntity
{
    public HashId Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public HashId? ParentId { get; private set; }
    public string Message { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public UserIdentityEntity Author { get; private set; } = null!;
    public ImmutableArray<FileChange> Changes { get; private set; }

    private CommitEntity()
    {
    }

    public static CommitEntity Create(
        HashId id,
        Guid projectId,
        HashId? parentId,
        string message,
        DateTimeOffset createdAt,
        UserIdentityEntity author,
        IEnumerable<FileChange> changes
    )
    {
        return new CommitEntity
        {
            Id = id,
            ProjectId = projectId,
            ParentId = parentId,
            Message = message,
            CreatedAt = createdAt,
            Author = author,
            Changes = changes.ToImmutableArray()
        };
    }
}