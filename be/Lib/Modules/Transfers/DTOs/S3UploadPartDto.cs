namespace Lib.Modules.Transfers.DTOs;

public record S3UploadPartDto(
    int PartNumber,
    string ETag
);