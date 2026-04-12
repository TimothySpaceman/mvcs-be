using Lib.Modules.Storages.Entities;

namespace Lib.Modules.Storages.Repositories;

public interface IStorageRepository
{
    public Task<Storage?> GetByIdAsync(Guid id);
    public Task<Storage?> GetByIdWithAccessAsync(Guid id, Guid userId);
    public Task<List<Storage>> GetAllByUserIdAsync(Guid userId);
    public Task<bool> ExistsByIdAsync(Guid id);
    public Task AddAsync(Storage storage);
    public void Delete(Storage storage);
    public Task AddAccessAsync(StorageAccess access);
    public void DeleteAccess(StorageAccess access);
    public Task SaveChangesAsync();
}