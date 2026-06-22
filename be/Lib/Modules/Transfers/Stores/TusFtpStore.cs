using System.IO.Pipelines;
using Lib.Infrastructure.Redis;
using Lib.Modules.Transfers.Adapters;
using Lib.Modules.Transfers.ConfigModels;
using Lib.Modules.Transfers.DTOs;
using Lib.Modules.Transfers.Factories;
using Lib.Modules.Transfers.Helpers;
using Lib.Shared.Exceptions;
using tusdotnet.Models.Configuration;

namespace Lib.Modules.Transfers.Stores;

public class TusFtpStore : IFullTusStore
{
    private const string PartSuffix = ".part";

    private readonly FtpStorageConfig _config;
    private readonly Guid _userId;
    private readonly string _scopePath;
    private readonly IRedisService _redisService;

    public TusFtpStore(
        FtpStorageConfig config,
        Guid userId,
        string scopePath,
        IRedisService redisService
    )
    {
        _config = config;
        _userId = userId;
        _scopePath = scopePath;
        _redisService = redisService;
    }

    public async Task<string> CreateFileAsync(long uploadLength, string metadata, CancellationToken cancellationToken)
    {
        var parsedMetadata = TusMetadataHelper.ParseMetadata(metadata);
        var fileName = parsedMetadata.TryGetValue("filename", out var fn) ? fn : Guid.NewGuid().ToString();

        var finalPath = FtpStorageAdapter.BuildFilePath(_config, _scopePath, fileName);
        var partPath = finalPath + PartSuffix;

        await using var client = RemoteFileClientFactory.Create(_config);
        await client.ConnectAsync(cancellationToken);

        await client.EnsureDirectoryAsync(GetDirectory(partPath), cancellationToken);
        if (await client.FileExistsAsync(partPath, cancellationToken))
        {
            await client.DeleteFileAsync(partPath, cancellationToken);
        }

        await client.AppendAsync(partPath, Stream.Null, cancellationToken);

        var id = Guid.NewGuid().ToString();
        var record = new FtpUploadRecordDto(finalPath, partPath, _userId, uploadLength, metadata);
        await _redisService.SetAsync(BuildRecordKey(id), record, TimeSpan.FromHours(6));

        return id;
    }

    public async Task<long> AppendDataAsync(string fileId, Stream stream, CancellationToken cancellationToken)
    {
        var record = await GetRecord(fileId);

        await using var client = RemoteFileClientFactory.Create(_config);
        await client.ConnectAsync(cancellationToken);

        var counting = new CountingStream(stream);
        await client.AppendAsync(record.PartPath, counting, cancellationToken);

        return counting.BytesRead;
    }

    public Task<long> AppendDataAsync(string fileId, PipeReader pipeReader, CancellationToken cancellationToken)
    {
        return AppendDataAsync(fileId, pipeReader.AsStream(), cancellationToken);
    }

    public async Task<long> GetUploadOffsetAsync(string fileId, CancellationToken cancellationToken)
    {
        var record = await GetRecord(fileId);

        await using var client = RemoteFileClientFactory.Create(_config);
        await client.ConnectAsync(cancellationToken);

        var size = await client.GetFileSizeAsync(record.PartPath, cancellationToken);
        return size < 0 ? 0 : size;
    }

    public async Task<long?> GetUploadLengthAsync(string fileId, CancellationToken cancellationToken)
    {
        var record = await GetRecord(fileId, throwIfNotFound: false);
        return record?.UploadLength;
    }

    public async Task<string> GetUploadMetadataAsync(string fileId, CancellationToken cancellationToken)
    {
        var record = await GetRecord(fileId);
        return record.TusMetadata;
    }

    public Task<bool> FileExistAsync(string fileId, CancellationToken cancellationToken)
    {
        return _redisService.ExistsAsync(BuildRecordKey(fileId));
    }

    public async Task DeleteFileAsync(string fileId, CancellationToken cancellationToken)
    {
        var record = await GetRecord(fileId, throwIfNotFound: false);
        if (record is null) return;

        await using var client = RemoteFileClientFactory.Create(_config);
        await client.ConnectAsync(cancellationToken);
        await client.DeleteFileAsync(record.PartPath, cancellationToken);

        await _redisService.DeleteAsync(BuildRecordKey(fileId));
    }

    public async Task CompleteUploadAsync(FileCompleteContext ctx)
    {
        var record = await GetRecord(ctx.FileId);

        await using var client = RemoteFileClientFactory.Create(_config);

        var finalSize = await client.GetFileSizeAsync(record.PartPath, ctx.CancellationToken);
        if (finalSize != record.UploadLength)
        {
            throw new InvalidOperationException(
                $"Upload size mismatch: expected {record.UploadLength}, got {finalSize}"
            );
        }

        await client.RenameAsync(record.PartPath, record.FinalPath, ctx.CancellationToken);
        await _redisService.DeleteAsync(BuildRecordKey(ctx.FileId));
    }

    private string BuildRecordKey(string fileId) => $"ftp-uploads:{_userId}:{fileId}";

    private async Task<FtpUploadRecordDto> GetRecord(string fileId)
    {
        return (await GetRecord(fileId, throwIfNotFound: true))!;
    }

    private async Task<FtpUploadRecordDto?> GetRecord(string fileId, bool throwIfNotFound)
    {
        var record = await _redisService.GetAsync<FtpUploadRecordDto>(BuildRecordKey(fileId));
        if (throwIfNotFound && record is null) throw new NotFoundException("Requested upload not found");
        return record;
    }

    private static string GetDirectory(string path)
    {
        var idx = path.LastIndexOf('/');
        return idx <= 0 ? "" : path[..idx];
    }
}

internal sealed class CountingStream : Stream
{
    private readonly Stream _inner;
    public long BytesRead { get; private set; }

    public CountingStream(Stream inner)
    {
        _inner = inner;
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var n = _inner.Read(buffer, offset, count);
        BytesRead += n;
        return n;
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken ct = default)
    {
        var n = await _inner.ReadAsync(buffer, ct);
        BytesRead += n;
        return n;
    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}