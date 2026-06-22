using System.Text.Json;
using Lib.Infrastructure.Redis;
using Lib.Modules.Storages.Entities;
using Lib.Modules.Transfers.Adapters;
using Lib.Modules.Transfers.ConfigModels;

namespace Lib.Modules.Transfers.Factories;

public class StorageAdapterFactory(IRedisService redisService) : IStorageAdapterFactory
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IStorageAdapter Create(Storage storage)
    {
        return storage.StorageType.Key switch
        {
            "aws-s3" => CreateS3Adapter(storage),
            "ftp" => CreateFtpAdapter(storage),
            var key => throw new NotSupportedException($"No storage adapter for storage type '{key}'")
        };
    }

    private T GetConfig<T>(string configString)
    {
        var config = JsonSerializer.Deserialize<T>(configString, JsonOptions);
        if (config is null) throw new InvalidOperationException("Failed to parse storage config");
        return config;
    }

    private S3StorageAdapter CreateS3Adapter(Storage storage)
    {
        return new S3StorageAdapter(GetConfig<S3StorageConfig>(storage.Config), redisService);
    }

    private FtpStorageAdapter CreateFtpAdapter(Storage storage)
    {
        return new FtpStorageAdapter(GetConfig<FtpStorageConfig>(storage.Config), redisService);
    }
}