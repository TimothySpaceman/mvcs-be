using Lib.Modules.Storages.Entities;

namespace Lib.Modules.Storages.Repositories;

public interface IStorageTypeRepository
{
    public Task<List<StorageType>> GetAllAsync();
    public Task<StorageType?> GetByIdAsync(Guid id);
}