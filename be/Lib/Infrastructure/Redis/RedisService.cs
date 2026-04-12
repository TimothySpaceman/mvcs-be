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

    public async Task<long> ListPushAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        var length = await _db.ListRightPushAsync((RedisKey)key, (RedisValue)json);
        if (expiry.HasValue) await _db.KeyExpireAsync((RedisKey)key, expiry.Value);
        return length;
    }

    public async Task<long> ListPushFrontAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        var length = await _db.ListLeftPushAsync((RedisKey)key, (RedisValue)json);
        if (expiry.HasValue) await _db.KeyExpireAsync((RedisKey)key, expiry.Value);
        return length;
    }

    public async Task<T?> ListPopAsync<T>(string key)
    {
        var redisValue = await _db.ListRightPopAsync((RedisKey)key);
        if (!redisValue.HasValue) return default;
        return JsonSerializer.Deserialize<T>((string)redisValue!, _jsonOptions);
    }

    public async Task<T?> ListPopFrontAsync<T>(string key)
    {
        var redisValue = await _db.ListLeftPopAsync((RedisKey)key);
        if (!redisValue.HasValue) return default;
        return JsonSerializer.Deserialize<T>((string)redisValue!, _jsonOptions);
    }

    public async Task<List<T>> ListGetAllAsync<T>(string key)
    {
        var values = await _db.ListRangeAsync((RedisKey)key, 0, -1);
        return values
            .Select(v => JsonSerializer.Deserialize<T>((string)v!, _jsonOptions)!)
            .ToList();
    }

    public async Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1)
    {
        var values = await _db.ListRangeAsync((RedisKey)key, start, stop);
        return values
            .Select(v => JsonSerializer.Deserialize<T>((string)v!, _jsonOptions)!)
            .ToList();
    }

    public Task<long> ListLengthAsync(string key)
    {
        return _db.ListLengthAsync((RedisKey)key);
    }

    public async Task<T?> ListGetByIndexAsync<T>(string key, long index)
    {
        var redisValue = await _db.ListGetByIndexAsync((RedisKey)key, index);
        if (!redisValue.HasValue) return default;
        return JsonSerializer.Deserialize<T>((string)redisValue!, _jsonOptions);
    }

    public async Task ListSetByIndexAsync<T>(string key, long index, T value)
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        await _db.ListSetByIndexAsync((RedisKey)key, index, (RedisValue)json);
    }

    public async Task<long> ListRemoveAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        return await _db.ListRemoveAsync((RedisKey)key, (RedisValue)json);
    }

    public Task ListTrimAsync(string key, long start, long stop)
    {
        return _db.ListTrimAsync((RedisKey)key, start, stop);
    }
}