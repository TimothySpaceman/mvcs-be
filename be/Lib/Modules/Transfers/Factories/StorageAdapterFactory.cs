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
            var key => throw new NotSupportedException($"No storage adapter for storage type '{key}'")
        };
    }

    private S3StorageAdapter CreateS3Adapter(Storage storage)
    {
        var config = JsonSerializer.Deserialize<S3StorageConfig>(storage.Config, JsonOptions);
        if (config is null)
        {
            throw new InvalidOperationException("Failed to parse S3 storage config");
        }

        return new S3StorageAdapter(config, redisService);
    }
}