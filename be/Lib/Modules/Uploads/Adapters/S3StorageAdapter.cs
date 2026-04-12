using Lib.Infrastructure.Redis;
using Lib.Modules.Uploads.ConfigModels;
using Lib.Modules.Uploads.Stores;

namespace Lib.Modules.Uploads.Adapters;

public class S3StorageAdapter(
    S3StorageConfig config,
    IRedisService redisService
) : IStorageAdapter
{
    public IFullTusStore CreateTusStore(Guid userId, string scopePath)
    {
        return new TusS3Store(config, userId, scopePath, redisService);
    }
}