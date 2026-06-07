namespace Lib.Modules.Vcs.DTOs;

public record PullResultDto(
    IEnumerable<CommitDto> Commits,
    IEnumerable<BlobMetadataDto> Blobs
);