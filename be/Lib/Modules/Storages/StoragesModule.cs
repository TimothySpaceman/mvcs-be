using Lib.Modules.Storages.Configurations;
using Lib.Modules.Storages.Repositories;
using Lib.Modules.Storages.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lib.Modules.Storages;

public static class StoragesModule
{
    public static IServiceCollection AddStoragesModule(this IServiceCollection services)
    {
        services.AddScoped<IStorageTypeRepository, StorageTypeRepository>();
        services.AddScoped<IStorageTypeService, StorageTypeService>();
        
        services.AddScoped<IStorageRepository, StorageRepository>();
        services.AddScoped<IStorageService, StorageService>();
 
        return services;
    }
 
    public static void ApplyStoragesConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new StorageTypeConfiguration());
        modelBuilder.ApplyConfiguration(new StorageConfiguration());
        modelBuilder.ApplyConfiguration(new StorageAccessConfiguration());
    }
}