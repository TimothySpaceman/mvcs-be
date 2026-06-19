using Lib.Modules.Releases.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lib.Modules.Releases;

public static class ReleasesModule
{
    public static IServiceCollection AddReleasesModule(this IServiceCollection services)
    {
        // services.AddScoped<ITaskRepository, TaskRepository>();
        // services.AddScoped<ITaskService, TaskService>();
        
        return services;
    }

    public static void ApplyReleasesConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ReleaseFileConfiguration());
        modelBuilder.ApplyConfiguration(new ReleaseConfiguration());
    }
}