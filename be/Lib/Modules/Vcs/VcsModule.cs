using Lib.Modules.Vcs.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lib.Modules.Vcs;

public static class VcsModule
{
    public static IServiceCollection AddVcsModule(this IServiceCollection services)
    {
        return services;
    }

    public static void ApplyVcsConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BlobMetadataEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CommitEntityConfiguration());
    }
}