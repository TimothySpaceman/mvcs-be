namespace Lib.Modules.Releases.DTOs;

public record CreateReleaseFileDto(
    string FileName,
    string BlobId
);