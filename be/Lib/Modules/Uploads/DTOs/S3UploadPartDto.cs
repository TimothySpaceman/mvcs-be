namespace Lib.Modules.Uploads.DTOs;

public record S3UploadPartDto(
    int PartNumber,
    string ETag
);