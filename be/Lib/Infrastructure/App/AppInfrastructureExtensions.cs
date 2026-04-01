using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lib.Infrastructure.App;

public static class AppInfrastructureExtensions
{
    public static IServiceCollection AddAppInfrastructure(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("AppDb"))
        );
        return services;
    }
}