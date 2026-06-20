using Lib.Modules.Transfers.Factories;
using Lib.Modules.Transfers.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lib.Modules.Transfers;

public static class TransfersModule
{
    public static IServiceCollection AddTransfersModule(this IServiceCollection services)
    {
        services.AddScoped<IStorageAdapterFactory, StorageAdapterFactory>();
        services.AddScoped<ITransferService, TransferService>();
 
        return services;
    }
}