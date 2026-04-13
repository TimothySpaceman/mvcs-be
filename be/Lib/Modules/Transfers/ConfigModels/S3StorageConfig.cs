namespace Lib.Modules.Transfers.ConfigModels;

public record S3StorageConfig(
    string Endpoint,
    string Region,
    string Bucket,
    string AccessKeyId,
    string SecretAccessKey,
    string RootPrefix = ""
);