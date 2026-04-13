using Lib.Infrastructure.Redis;
using Lib.Modules.Transfers.ConfigModels;
using Lib.Modules.Transfers.Stores;

namespace Lib.Modules.Transfers.Adapters;

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