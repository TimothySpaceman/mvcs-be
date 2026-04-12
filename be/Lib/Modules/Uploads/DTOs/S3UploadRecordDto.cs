namespace Lib.Modules.Uploads.DTOs;

public record S3UploadRecordDto(
    string S3UploadId,
    string S3Key,
    Guid UserId,
    long UploadLength,
    long Offset,
    string TusMetadata
);