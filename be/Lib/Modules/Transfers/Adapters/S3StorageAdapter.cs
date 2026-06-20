using System.Net;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Lib.Infrastructure.Redis;
using Lib.Modules.Transfers.ConfigModels;
using Lib.Modules.Transfers.DTOs;
using Lib.Modules.Transfers.Stores;
using Lib.Shared.Exceptions;

namespace Lib.Modules.Transfers.Adapters;

public class S3StorageAdapter : IStorageAdapter
{
    private readonly S3StorageConfig _config;
    private readonly IRedisService _redisService;
    private readonly AmazonS3Client _s3Client;

    public S3StorageAdapter(S3StorageConfig config, IRedisService redisService)
    {
        _config = config;
        _redisService = redisService;
        _s3Client = CreateS3Client();
    }

    public IFullTusStore CreateTusStore(Guid userId, string scopePath)
    {
        return new TusS3Store(_config, _s3Client, userId, scopePath, _redisService);
    }

    public async Task<long> GetContentLengthAsync(
        string filePath,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _config.Bucket,
                Key = filePath,
            };

            var response = await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
            return response.ContentLength;
        }
        catch (AmazonS3Exception ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException("File not found");
            }

            throw new Exception("Failed to get content size from storage");
        }
    }

    public async Task<Stream> GetContentAsync(
        string filePath,
        ByteRange? range,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _config.Bucket,
                Key = filePath
            };

            if (range is not null) request.ByteRange = new Amazon.S3.Model.ByteRange(range.ToHeaderValue());

            var response = await _s3Client.GetObjectAsync(request, cancellationToken);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException("File not found");
            }

            if (ex.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
            {
                throw new RangeNotSatisfiableException("Requested range not satisfiable");
            }

            throw new Exception("Failed to fetch content from storage");
        }
    }

    public async Task<StorageHealthDto> GetStorageHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _config.Bucket,
                MaxKeys = 1
            };
            await _s3Client.ListObjectsV2Async(request, cancellationToken);
            return new StorageHealthDto(true, null);
        }
        catch (AmazonS3Exception ex)
        {
            return new StorageHealthDto(false, ex.ErrorCode switch
            {
                "NoSuchBucket" => "Bucket not found",
                "AccessDenied" => "Access denied",
                _ => "Storage is unavailable"
            });
        }
        catch (Exception)
        {
            return new StorageHealthDto(false, "Storage is unavailable");
        }
    }

    public static string BuildFilePath(
        S3StorageConfig config,
        string scopePath,
        string fileName
    )
    {
        var parts = new List<string>();

        if (!string.IsNullOrEmpty(config.RootPrefix))
        {
            parts.Add(config.RootPrefix.Trim('/'));
        }

        if (!string.IsNullOrEmpty(scopePath))
        {
            parts.Add(scopePath.Trim('/'));
        }

        parts.Add(fileName);

        return string.Join("/", parts);
    }

    private AmazonS3Client CreateS3Client()
    {
        var credentials = new BasicAWSCredentials(_config.AccessKeyId, _config.SecretAccessKey);

        var s3Config = new AmazonS3Config
        {
            ServiceURL = _config.Endpoint,
            AuthenticationRegion = _config.Region,
            RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED
        };

        return new AmazonS3Client(credentials, s3Config);
    }
}