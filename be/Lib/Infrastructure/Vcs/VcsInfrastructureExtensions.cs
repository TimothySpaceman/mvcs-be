using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lib.Infrastructure.Vcs;

public static class VcsInfrastructureExtensions
{
    public static IServiceCollection AddVcsInfrastructure(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddDbContext<VcsDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("VcsDb"))
        );
        return services;
    }
}