using Core.Commits;
using Core.Storage;

namespace Lib.Modules.Vcs.DTOs;

public record CommitInfoDto(
    HashId Id,
    HashId? ParentId,
    HashId? SecondParentId,
    CommitKind Kind,
    string Message,
    UserIdentityDto Author,
    DateTimeOffset CreatedAt
)
{
    public static CommitInfoDto FromDomain(Commit domain)
    {
        return new CommitInfoDto(
            domain.Id,
            domain.ParentId,
            domain.SecondParentId,
            domain.Kind,
            domain.Message,
            UserIdentityDto.FromDomain(domain.Author),
            domain.CreatedAt
        );
    }
}