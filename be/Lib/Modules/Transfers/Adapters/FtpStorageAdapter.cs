using Lib.Infrastructure.Redis;
using Lib.Modules.Transfers.ConfigModels;
using Lib.Modules.Transfers.DTOs;
using Lib.Modules.Transfers.Factories;
using Lib.Modules.Transfers.Stores;
using Lib.Modules.Transfers.Utils;
using Lib.Shared.Exceptions;

namespace Lib.Modules.Transfers.Adapters;

public class FtpStorageAdapter : IStorageAdapter
{
    private readonly FtpStorageConfig _config;
    private readonly IRedisService _redisService;

    public FtpStorageAdapter(FtpStorageConfig config, IRedisService redisService)
    {
        _config = config;
        _redisService = redisService;
    }

    public IFullTusStore CreateTusStore(Guid userId, string scopePath)
    {
        return new TusFtpStore(_config, userId, scopePath, _redisService);
    }

    public async Task<long> GetContentLengthAsync(
        string filePath,
        CancellationToken cancellationToken = default
    )
    {
        await using var client = RemoteFileClientFactory.Create(_config);
        await client.ConnectAsync(cancellationToken);

        var size = await client.GetFileSizeAsync(filePath, cancellationToken);
        if (size < 0) throw new NotFoundException("File not found");
        return size;
    }

    public async Task<Stream> GetContentAsync(
        string filePath,
        ByteRange? range,
        CancellationToken cancellationToken = default
    )
    {
        var client = RemoteFileClientFactory.Create(_config);
        try
        {
            await client.ConnectAsync(cancellationToken);

            var size = await client.GetFileSizeAsync(filePath, cancellationToken);
            if (size < 0) throw new NotFoundException("File not found");

            var start = range?.Start ?? 0;
            if (start < 0 || start > size)
            {
                throw new RangeNotSatisfiableException("Requested range not satisfiable");
            }
            
            var endInclusive = range?.End ?? size - 1;
            if (endInclusive >= size) endInclusive = size - 1;
            if (endInclusive < start)
            {
                throw new RangeNotSatisfiableException("Requested range not satisfiable");
            }

            var length = endInclusive - start + 1;
            var inner = await client.OpenReadAsync(filePath, start, cancellationToken);
            
            return new RemoteRangeStream(inner, client, length);
        }
        catch
        {
            await client.DisposeAsync();
            throw;
        }
    }

    public async Task<StorageHealthDto> GetStorageHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var client = RemoteFileClientFactory.Create(_config);
            await client.ConnectAsync(cancellationToken);
            await client.FileExistsAsync(NormalizeRoot(_config.RootPath), cancellationToken);
            return new StorageHealthDto(true, null);
        }
        catch (Exception ex)
        {
            return new StorageHealthDto(false, ex switch
            {
                Renci.SshNet.Common.SshAuthenticationException => "Authentication failed",
                Renci.SshNet.Common.SshConnectionException => "Connection failed",
                FluentFTP.Exceptions.FtpAuthenticationException => "Authentication failed",
                System.Net.Sockets.SocketException => "Host unreachable",
                _ => "Storage is unavailable"
            });
        }
    }

    public static string BuildFilePath(FtpStorageConfig config, string scopePath, string fileName)
    {
        var parts = new List<string>();

        if (!string.IsNullOrEmpty(config.RootPath)) parts.Add(config.RootPath.Trim('/'));
        if (!string.IsNullOrEmpty(scopePath)) parts.Add(scopePath.Trim('/'));
        parts.Add(fileName.Trim('/'));

        var joined = string.Join("/", parts.Where(p => !string.IsNullOrEmpty(p)));
        
        return config.RootPath.StartsWith('/') ? "/" + joined : joined;
    }

    private static string NormalizeRoot(string root) => string.IsNullOrEmpty(root) ? "/" : root;
}