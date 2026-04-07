using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Lib.Infrastructure.Redis;

public static class RedisExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Redis");
        if (connectionString is null)
        {
            throw new InvalidOperationException("Redis connection string 'Redis' is not configured.");
        }

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(connectionString)
        );

        services.AddSingleton<IRedisService, RedisService>();

        return services;
    }
}