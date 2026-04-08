using Lib.Modules.Storages.DTOs;

namespace Lib.Modules.Storages.Services;

public interface IStorageTypeService
{
    Task<List<StorageTypeInfoDto>> GetAllAsync();
    Task<StorageTypeDto?> GetByIdAsync(Guid id);
}
