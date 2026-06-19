using Lib.Modules.Releases.Configurations;
using Lib.Modules.Releases.Repositories;
using Lib.Modules.Releases.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lib.Modules.Releases;

public static class ReleasesModule
{
    public static IServiceCollection AddReleasesModule(this IServiceCollection services)
    {
        services.AddScoped<IReleaseRepository, ReleaseRepository>();
        services.AddScoped<IReleaseService, ReleaseService>();
        
        return services;
    }

    public static void ApplyReleasesConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ReleaseFileConfiguration());
        modelBuilder.ApplyConfiguration(new ReleaseConfiguration());
    }
}