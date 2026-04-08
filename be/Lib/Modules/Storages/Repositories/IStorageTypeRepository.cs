using Lib.Modules.Storages.Entities;

namespace Lib.Modules.Storages.Repositories;

public interface IStorageTypeRepository
{
    Task<List<StorageType>> GetAllAsync();
    Task<StorageType?> GetByIdAsync(Guid id);
}