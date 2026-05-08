using Core.Commits;
using Core.Storage;
using Lib.Modules.Vcs.Entities;

namespace Lib.Modules.Vcs.Mappings;

public static class CommitMapping
{
    public static CommitEntity ToEntity(this Commit domain, Guid projectId)
    {
        var commitId = domain.Id.Bytes.ToArray();

        return CommitEntity.Create(
            commitId,
            projectId,
            domain.ParentId?.Bytes.ToArray(),
            domain.Message,
            domain.CreatedAt,
            domain.Author.ToEntity(),
            domain.Changes
        );
    }

    public static Commit ToDomain(this CommitEntity entity)
    {
        return new Commit(
            new HashId(entity.Id),
            entity.ParentId is null ? null : new HashId(entity.ParentId),
            entity.Message,
            entity.Changes,
            entity.Author.ToDomain(),
            entity.CreatedAt
        );
    }
}