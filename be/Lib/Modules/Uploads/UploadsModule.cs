using Lib.Modules.Uploads.Endpoints;
using Lib.Modules.Uploads.Factories;
using Lib.Modules.Uploads.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using tusdotnet;

namespace Lib.Modules.Uploads;

public static class UploadsModule
{
    public static IServiceCollection AddUploadsModule(this IServiceCollection services)
    {
        services.AddScoped<IStorageAdapterFactory, StorageAdapterFactory>();
        services.AddScoped<IUploadService, UploadService>();
 
        return services;
    }
    
    public static WebApplication MapUploadsModule(this WebApplication app)
    {
        app.MapTus("/api/storages/{storageId}/uploads", TusEndpoints.StorageUploads);
        return app;
    }
}