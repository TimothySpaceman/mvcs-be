namespace Lib.Modules.Vcs.DTOs;

public record PushRequestDto(
    string RefName,
    string ExpectedHead,
    IEnumerable<CommitDto> Commits
);