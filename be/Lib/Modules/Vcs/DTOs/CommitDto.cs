using Core.Commits;
using Core.Storage;

namespace Lib.Modules.Vcs.DTOs;

public record CommitDto(
    HashId Id,
    HashId? ParentId,
    string Message,
    IReadOnlyList<FileChangeDto> Changes,
    UserIdentityDto Author,
    DateTimeOffset CreatedAt
)
{
    public static CommitDto FromDomain(Commit domain)
    {
        return new CommitDto(
            domain.Id,
            domain.ParentId,
            domain.Message,
            domain.Changes.Select(FileChangeDto.FromDomain).ToArray(),
            UserIdentityDto.FromDomain(domain.Author),
            domain.CreatedAt
        );
    }
}