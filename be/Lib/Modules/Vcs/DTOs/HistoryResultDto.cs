namespace Lib.Modules.Vcs.DTOs;

public record HistoryResultDto(
    IEnumerable<CommitInfoDto> Commits
);