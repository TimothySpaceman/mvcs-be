namespace Lib.Infrastructure.Redis;

public interface IRedisService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<bool> ExistsAsync(string key);
    Task<bool> DeleteAsync(string key);
}