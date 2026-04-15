namespace Lib.Infrastructure.Redis;

public interface IRedisService
{
    public Task<T?> GetAsync<T>(string key);
    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    public Task<bool> ExistsAsync(string key);
    public Task<bool> DeleteAsync(string key);

    public Task<long> ListPushAsync<T>(string key, T value, TimeSpan? expiry = null);
    public Task<long> ListPushFrontAsync<T>(string key, T value, TimeSpan? expiry = null);
    public Task<T?> ListPopAsync<T>(string key);
    public Task<T?> ListPopFrontAsync<T>(string key);
    public Task<List<T>> ListGetAllAsync<T>(string key);
    public Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1);
    public Task<long> ListLengthAsync(string key);
    public Task<T?> ListGetByIndexAsync<T>(string key, long index);
    public Task ListSetByIndexAsync<T>(string key, long index, T value);
    public Task<long> ListRemoveAsync<T>(string key, T value);
    public Task ListTrimAsync(string key, long start, long stop);
}