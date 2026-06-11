using Core.Storage;

namespace Lib.Modules.Vcs.DTOs;

public record PushRequestBodyDto(
    string RefName,
    HashId? ExpectedHead,
    IEnumerable<CommitDto> Commits
);