namespace Lib.Modules.Vcs.DTOs;

public record PullResultBodyDto(
    IEnumerable<CommitDto> Commits,
    IEnumerable<BlobMetadataDto> Blobs
);