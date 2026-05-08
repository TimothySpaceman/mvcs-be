using System.Collections.Immutable;
using Core.FileChanges;

namespace Lib.Modules.Vcs.Entities;

public class CommitEntity
{
    public byte[] Id { get; private set; } = null!;
    public Guid ProjectId { get; private set; }
    public byte[]? ParentId { get; private set; }
    public string Message { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public UserIdentityEntity Author { get; private set; } = null!;
    public ImmutableArray<FileChange> Changes { get; private set; }

    private CommitEntity()
    {
    }

    public static CommitEntity Create(
        byte[] id,
        Guid projectId,
        byte[]? parentId,
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