using Lib.Modules.Storages.DTOs;

namespace Lib.Modules.Storages.Services;

public interface IStorageTypeService
{
    public Task<List<StorageTypeInfoDto>> GetAllAsync();
    public Task<StorageTypeDto?> GetByIdAsync(Guid id);
}