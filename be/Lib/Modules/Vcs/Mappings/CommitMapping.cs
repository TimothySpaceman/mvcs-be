using Core.Commits;
using Lib.Modules.Vcs.Entities;

namespace Lib.Modules.Vcs.Mappings;

public static class CommitMapping
{
    public static CommitEntity ToEntity(this Commit domain, Guid projectId)
    {
        return CommitEntity.Create(
            domain.Id,
            projectId,
            domain.ParentId,
            domain.Message,
            domain.CreatedAt,
            domain.Author.ToEntity(),
            domain.Changes
        );
    }

    public static Commit ToDomain(this CommitEntity entity)
    {
        return new Commit(
            entity.Id,
            entity.ParentId,
            entity.Message,
            entity.Changes,
            entity.Author.ToDomain(),
            entity.CreatedAt
        );
    }
}