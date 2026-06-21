namespace Lib.Modules.Transfers.DTOs;

public record FtpUploadRecordDto(
    string FinalPath,
    string PartPath,
    Guid UserId,
    long UploadLength,
    string TusMetadata
);