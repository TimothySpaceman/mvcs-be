using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Lib.Modules.Uploads.ConfigModels;
using Microsoft.Extensions.Logging;
using tusdotnet.Interfaces;
using tusdotnet.Stores.S3;

namespace Lib.Modules.Uploads.Adapters;

public class S3StorageAdapter(S3StorageConfig config, ILoggerFactory loggerFactory) : IStorageAdapter
{
    public ITusStore CreateTusStore()
    {
        var credentials = new BasicAWSCredentials(config.AccessKeyId, config.SecretAccessKey);

        var s3Config = new AmazonS3Config
        {
            ServiceURL = config.Endpoint,
            AuthenticationRegion = config.Region,
            RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED
        };

        var storeConfig = new TusS3StoreConfiguration
        {
            BucketName = config.Bucket
        };

        var logger = loggerFactory.CreateLogger<TusS3Store>();
        
        return new TusS3Store(logger, storeConfig, credentials, s3Config);
    }
    
    public string BuildFinalKey(Guid userId, string scopePath, string fileName)
    {
        var parts = new List<string>();

        if (!string.IsNullOrEmpty(config.RootPrefix))
        {
            parts.Add(config.RootPrefix.Trim('/'));
        }

        parts.Add($"users/{userId}");

        if (!string.IsNullOrEmpty(scopePath))
        {
            parts.Add(scopePath.Trim('/'));
        }

        parts.Add(fileName);

        return string.Join("/", parts);
    }

    public async Task MoveFileAsync(string sourceKey, string destKey, CancellationToken cancellationToken)
    {
        var credentials = new BasicAWSCredentials(config.AccessKeyId, config.SecretAccessKey);

        using var s3 = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = config.Endpoint,
            AuthenticationRegion = config.Region,
            RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED
        });

        await s3.CopyObjectAsync(new CopyObjectRequest
        {
            SourceBucket = config.Bucket,
            SourceKey = sourceKey,
            DestinationBucket = config.Bucket,
            DestinationKey = destKey
        }, cancellationToken);

        await s3.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = config.Bucket,
            Key = $"upload_info/{sourceKey}"
        }, cancellationToken);
    }
}