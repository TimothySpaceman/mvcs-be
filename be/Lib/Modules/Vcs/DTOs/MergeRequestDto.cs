using Core.Storage;
using Lib.Modules.Vcs.Entities;

namespace Lib.Modules.Vcs.DTOs;

public record MergeRequestDto(
    Guid Id,
    Guid AuthorId,
    string Title,
    string TargetRefName,
    string SourceRefName,
    HashId MergeCommitId,
    DateTimeOffset CreatedAt
)
{
    public static MergeRequestDto FromDomain(MergeRequest domain)
    {
        return new MergeRequestDto(
            domain.Id,
            domain.AuthorId,
            domain.Title,
            domain.TargetRefName,
            domain.SourceRefName,
            domain.MergeCommitId,
            domain.CreatedAt
        );
    }
}