using System.Text.Json;
using StackExchange.Redis;

namespace Lib.Infrastructure.Redis;

public class RedisService : IRedisService
{
    private readonly IDatabase _db;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var redisValue = await _db.StringGetAsync((RedisKey)key);
        if (!redisValue.HasValue) return default;

        var json = (string)redisValue!;
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        if (expiry.HasValue)
        {
            await _db.StringSetAsync((RedisKey)key, (RedisValue)json, new Expiration(expiry.Value));
        }
        else
        {
            await _db.StringSetAsync((RedisKey)key, (RedisValue)json);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _db.KeyExistsAsync(key);
    }

    public async Task<bool> DeleteAsync(string key)
    {
        return await _db.KeyDeleteAsync(key);
    }
}