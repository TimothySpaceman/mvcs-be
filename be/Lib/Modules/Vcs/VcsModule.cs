using Core.Storage;
using Lib.Modules.Vcs.Configurations;
using Lib.Modules.Vcs.Converters;
using Lib.Modules.Vcs.Endpoints;
using Lib.Modules.Vcs.Repository;
using Lib.Modules.Vcs.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tusdotnet;

namespace Lib.Modules.Vcs;

public static class VcsModule
{
    public static IServiceCollection AddVcsModule(this IServiceCollection services)
    {
        services.AddScoped<IBlobMetadataRepository, BlobMetadataRepository>();
        services.AddScoped<IBlobTransferService, BlobTransferService>();
        return services;
    }

    public static WebApplication MapVcsModule(this WebApplication app)
    {
        app.MapTus("/api/projects/{projectId}/blobs/uploads", BlobTusEndpoints.BlobUploads);
        return app;
    }

    public static void ApplyVcsConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BlobMetadataEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CommitEntityConfiguration());
    }
    
    public static void ApplyVcsConventions(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<HashId>()
            .HaveConversion<HashIdValueConverter>()
            .HaveMaxLength(32);
    }
}