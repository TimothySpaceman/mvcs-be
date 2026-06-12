using Core.Storage;
using Lib.Modules.Vcs.Configurations;
using Lib.Modules.Vcs.Controllers;
using Lib.Modules.Vcs.Converters;
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

        services.AddScoped<ICommitRepository, CommitRepository>();
        services.AddScoped<ICommitService, CommitService>();

        services.AddScoped<IRefRepository, RefRepository>();
        services.AddScoped<IRefService, RefService>();
        
        services.AddScoped<IMergeRequestRepository, MergeRequestRepository>();

        services.AddScoped<IPushService, PushService>();
        services.AddScoped<IMergeService, MergeService>();
        
        services.AddScoped<ISnapshotMetadataRepository, SnapshotMetadataRepository>();
        services.AddScoped<ISnapshotMetadataService, SnapshotMetadataService>();

        return services;
    }

    public static WebApplication MapVcsModule(this WebApplication app)
    {
        app.MapTus("/api/projects/{projectId}/blobs/uploads", BlobController.BlobUploads);
        return app;
    }

    public static void ApplyVcsConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BlobMetadataEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CommitEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RefEntityConfiguration());
        modelBuilder.ApplyConfiguration(new MergeRequestConfiguration());
        modelBuilder.ApplyConfiguration(new SnapshotMetadataConfiguration());
    }

    public static void ApplyVcsConventions(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<HashId>()
            .HaveConversion<HashIdValueConverter>()
            .HaveMaxLength(32);
    }
}