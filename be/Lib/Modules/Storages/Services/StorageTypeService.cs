using Lib.Modules.Storages.DTOs;
using Lib.Modules.Storages.Repositories;

namespace Lib.Modules.Storages.Services;

public class StorageTypeService(IStorageTypeRepository repository) : IStorageTypeService
{
    public async Task<List<StorageTypeInfoDto>> GetAllAsync()
    {
        var types = await repository.GetAllAsync();
        return types.Select(StorageTypeInfoDto.FromStorageType).ToList();
    }

    public async Task<StorageTypeDto?> GetByIdAsync(Guid id)
    {
        var type = await repository.GetByIdAsync(id);
        return type is null ? null : StorageTypeDto.FromStorageType(type);
    }
}