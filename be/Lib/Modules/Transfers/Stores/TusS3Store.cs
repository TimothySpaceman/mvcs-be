using System.IO.Pipelines;
using System.Text;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Lib.Infrastructure.Redis;
using Lib.Modules.Transfers.ConfigModels;
using Lib.Modules.Transfers.DTOs;
using Lib.Shared.Exceptions;
using tusdotnet.Models.Configuration;

namespace Lib.Modules.Transfers.Stores;

public class TusS3Store : IFullTusStore, IDisposable
{
    private readonly S3StorageConfig _config;
    private readonly Guid _userId;
    private readonly string _scopePath;
    private readonly IRedisService _redisService;
    private readonly AmazonS3Client _s3Client;

    public TusS3Store(S3StorageConfig config, Guid userId, string scopePath, IRedisService redisService)
    {
        _config = config;
        _userId = userId;
        _scopePath = scopePath;
        _redisService = redisService;
        _s3Client = CreateS3Client(config);
    }

    public async Task<string> CreateFileAsync(long uploadLength, string metadata, CancellationToken cancellationToken)
    {
        var parsedMetadata = ParseMetadata(metadata);
        var fileName = parsedMetadata.TryGetValue("filename", out var fn) ? fn : Guid.NewGuid().ToString();

        var request = new InitiateMultipartUploadRequest
        {
            BucketName = _config.Bucket,
            Key = BuildFilePath(fileName)
        };
        var response = await _s3Client.InitiateMultipartUploadAsync(request, cancellationToken);

        var uploadRecord = new S3UploadRecordDto(
            response.UploadId,
            response.Key,
            _userId,
            uploadLength,
            0,
            metadata
        );

        var id = Guid.NewGuid().ToString();
        await _redisService.SetAsync(BuildUploadRecordKey(id), uploadRecord, TimeSpan.FromMinutes(30));

        return id;
    }

    public async Task<long?> GetUploadLengthAsync(string fileId, CancellationToken cancellationToken)
    {
        var record = await GetUploadRecord(fileId, false);
        return record?.UploadLength;
    }

    public async Task<long> GetUploadOffsetAsync(string fileId, CancellationToken cancellationToken)
    {
        var record = await GetUploadRecord(fileId);
        return record.Offset;
    }

    public async Task<string> GetUploadMetadataAsync(string fileId, CancellationToken cancellationToken)
    {
        var record = await GetUploadRecord(fileId);
        return record.TusMetadata;
    }

    public Task<bool> FileExistAsync(string fileId, CancellationToken cancellationToken)
    {
        return _redisService.ExistsAsync(BuildUploadRecordKey(fileId));
    }

    public async Task<long> AppendDataAsync(string fileId, Stream stream, CancellationToken cancellationToken)
    {
        var record = await GetUploadRecord(fileId);
        var partsKey = BuildUploadPartsKey(fileId);
        var partNumber = (int)await _redisService.ListLengthAsync(partsKey) + 1;

        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer, cancellationToken);
        buffer.Position = 0;
        var bytesWritten = buffer.Length;

        var request = new UploadPartRequest
        {
            BucketName = _config.Bucket,
            Key = record.S3Key,
            UploadId = record.S3UploadId,
            PartNumber = partNumber,
            InputStream = buffer,
        };
        var response = await _s3Client.UploadPartAsync(request, cancellationToken);

        var partRecord = new S3UploadPartDto(
            partNumber,
            response.ETag
        );
        await _redisService.ListPushAsync(partsKey, partRecord, TimeSpan.FromMinutes(30));

        await _redisService.SetAsync(
            BuildUploadRecordKey(fileId),
            record with { Offset = record.Offset + bytesWritten },
            TimeSpan.FromMinutes(30)
        );

        return bytesWritten;
    }

    public Task<long> AppendDataAsync(string fileId, PipeReader pipeReader, CancellationToken cancellationToken)
    {
        return AppendDataAsync(fileId, pipeReader.AsStream(), cancellationToken);
    }

    public async Task DeleteFileAsync(string fileId, CancellationToken cancellationToken)
    {
        var record = await GetUploadRecord(fileId);

        var request = new AbortMultipartUploadRequest
        {
            BucketName = _config.Bucket,
            Key = record.S3Key,
            UploadId = record.S3UploadId
        };
        await _s3Client.AbortMultipartUploadAsync(request, cancellationToken);

        await _redisService.DeleteAsync(BuildUploadRecordKey(fileId));
        await _redisService.DeleteAsync(BuildUploadPartsKey(fileId));
    }

    public async Task CompleteUploadAsync(FileCompleteContext ctx)
    {
        var record = await GetUploadRecord(ctx.FileId);

        var request = new CompleteMultipartUploadRequest
        {
            BucketName = _config.Bucket,
            Key = record.S3Key,
            UploadId = record.S3UploadId,
        };

        var partsKey = BuildUploadPartsKey(ctx.FileId);
        var parts = await _redisService.ListGetAllAsync<S3UploadPartDto>(partsKey);
        request.AddPartETags(parts.Select(p => new PartETag(p.PartNumber, p.ETag)));

        await _s3Client.CompleteMultipartUploadAsync(request, ctx.CancellationToken);
        await _redisService.DeleteAsync(BuildUploadRecordKey(ctx.FileId));
        await _redisService.DeleteAsync(BuildUploadPartsKey(ctx.FileId));
    }

    private static string DecodeMetadataValue(string base64Value)
    {
        var bytes = Convert.FromBase64String(base64Value);
        return Encoding.UTF8.GetString(bytes);
    }

    private static Dictionary<string, string> ParseMetadata(string metadata)
    {
        return metadata
            .Split(',')
            .Select(pair => pair.Trim().Split(' '))
            .Where(parts => parts.Length == 2)
            .ToDictionary(
                parts => parts[0],
                parts => DecodeMetadataValue(parts[1])
            );
    }

    private string BuildFilePath(string fileName)
    {
        var parts = new List<string>();

        if (!string.IsNullOrEmpty(_config.RootPrefix))
        {
            parts.Add(_config.RootPrefix.Trim('/'));
        }

        parts.Add($"users/{_userId}");

        if (!string.IsNullOrEmpty(_scopePath))
        {
            parts.Add(_scopePath.Trim('/'));
        }

        parts.Add(fileName);

        return string.Join("/", parts);
    }

    private string BuildUploadRecordKey(string fileId)
    {
        return $"uploads:{_userId}:{fileId}";
    }

    private string BuildUploadPartsKey(string fileId)
    {
        return $"uploads:{_userId}:{fileId}:parts";
    }

    private async Task<S3UploadRecordDto> GetUploadRecord(string fileId)
    {
        return (await GetUploadRecord(fileId, true))!;
    }

    private async Task<S3UploadRecordDto?> GetUploadRecord(string fileId, bool throwIfNotFound)
    {
        var key = BuildUploadRecordKey(fileId);
        var record = await _redisService.GetAsync<S3UploadRecordDto>(key);
        if (throwIfNotFound && record is null) throw new NotFoundException("Requested upload not found");
        return record;
    }

    private AmazonS3Client CreateS3Client(S3StorageConfig config)
    {
        var credentials = new BasicAWSCredentials(config.AccessKeyId, config.SecretAccessKey);

        var s3Config = new AmazonS3Config
        {
            ServiceURL = config.Endpoint,
            AuthenticationRegion = config.Region,
            RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED
        };

        return new AmazonS3Client(credentials, s3Config);
    }

    public void Dispose()
    {
        _s3Client.Dispose();
    }
}