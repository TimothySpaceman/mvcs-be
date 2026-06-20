using System.Collections.Immutable;
using Core.Commits;
using Core.Storage;

namespace Lib.Modules.Vcs.DTOs;

public record CommitDto(
    HashId Id,
    HashId? ParentId,
    HashId? SecondParentId,
    CommitKind Kind,
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
            domain.SecondParentId,
            domain.Kind,
            domain.Message,
            domain.Changes.Select(FileChangeDto.FromDomain).ToArray(),
            UserIdentityDto.FromDomain(domain.Author),
            domain.CreatedAt
        );
    }

    public Commit ToDomain()
    {
        return new Commit(
            Id,
            ParentId,
            SecondParentId,
            Kind,
            Message,
            Changes.Select(c => c.ToDomain()).ToImmutableArray(),
            Author.ToDomain(),
            CreatedAt
        );
    }
}