using Lib.Infrastructure.App;
using Lib.Modules.Storages.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Storages.Repositories;

public class StorageTypeRepository(AppDbContext db) : IStorageTypeRepository
{
    public Task<List<StorageType>> GetAllAsync()
    {
        return db.Set<StorageType>().ToListAsync();
    }
 
    public Task<StorageType?> GetByIdAsync(Guid id)
    {
        return db.Set<StorageType>().FirstOrDefaultAsync(t => t.Id == id);
    }
}
