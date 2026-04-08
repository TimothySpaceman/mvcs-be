using Lib.Modules.Storages.Entities;

namespace Lib.Modules.Storages.Repositories;

public interface IStorageRepository
{
    Task<Storage?> GetByIdAsync(Guid id);
    Task<Storage?> GetByIdWithAccessAsync(Guid id, Guid userId);
    Task<List<Storage>> GetAllByUserIdAsync(Guid userId);
    Task<bool> ExistsByIdAsync(Guid id);
    Task AddAsync(Storage storage);
    void Delete(Storage storage);
    Task AddAccessAsync(StorageAccess access);
    void DeleteAccess(StorageAccess access);
    Task SaveChangesAsync();
}
