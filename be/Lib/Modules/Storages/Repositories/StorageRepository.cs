using Lib.Infrastructure.App;
using Lib.Modules.Storages.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Storages.Repositories;

public class StorageRepository(AppDbContext db) : IStorageRepository
{
    public Task<Storage?> GetByIdAsync(Guid id)
    {
        return db.Set<Storage>()
            .Include(s => s.StorageType)
            .Include(s => s.AccessEntries)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public Task<List<Storage>> GetAllByUserIdAsync(Guid userId)
    {
        return db.Set<Storage>()
            .Include(s => s.StorageType)
            .Include(s => s.AccessEntries)
            .Where(s => s.IsPublic || s.AccessEntries.Any(a => a.UserId == userId))
            .ToListAsync();
    }

    public Task<bool> ExistsByIdAsync(Guid id)
    {
        return db.Set<Storage>().AnyAsync(s => s.Id == id);
    }

    public async Task AddAsync(Storage storage)
    {
        await db.Set<Storage>().AddAsync(storage);
    }

    public void Delete(Storage storage)
    {
        db.Set<Storage>().Remove(storage);
    }

    public async Task AddAccessAsync(StorageAccess access)
    {
        await db.Set<StorageAccess>().AddAsync(access);
    }

    public void DeleteAccess(StorageAccess access)
    {
        db.Set<StorageAccess>().Remove(access);
    }

    public Task SaveChangesAsync()
    {
        return db.SaveChangesAsync();
    }
}