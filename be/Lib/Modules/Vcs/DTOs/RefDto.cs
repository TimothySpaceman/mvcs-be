using Core.Storage;
using Lib.Modules.Vcs.Entities;

namespace Lib.Modules.Vcs.DTOs;

public record RefDto(
    string Name,
    HashId? CommitId
)
{
    public static RefDto FromEntity(RefEntity entity)
    {
        return new RefDto(entity.Name, entity.CommitId);
    }
}