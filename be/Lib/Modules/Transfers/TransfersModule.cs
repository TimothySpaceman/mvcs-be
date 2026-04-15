using Lib.Modules.Transfers.Endpoints;
using Lib.Modules.Transfers.Factories;
using Lib.Modules.Transfers.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using tusdotnet;

namespace Lib.Modules.Transfers;

public static class TransfersModule
{
    public static IServiceCollection AddTransfersModule(this IServiceCollection services)
    {
        services.AddScoped<IStorageAdapterFactory, StorageAdapterFactory>();
        services.AddScoped<ITransferService, TransferService>();
 
        return services;
    }
    
    public static WebApplication MapTransfersModule(this WebApplication app)
    {
        app.MapTus("/api/storages/{storageId}/uploads", TusEndpoints.StorageUploads);
        return app;
    }
}