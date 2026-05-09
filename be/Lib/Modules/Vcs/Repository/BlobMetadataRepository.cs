using Core.Storage;
using Lib.Infrastructure.Vcs;
using Lib.Modules.Vcs.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Vcs.Repository;

public class BlobMetadataRepository(VcsDbContext db) : IBlobMetadataRepository
{
    public Task<BlobMetadataEntity?> GetByIdAsync(HashId id, Guid projectId)
    {
        return db.Set<BlobMetadataEntity>()
            .FirstOrDefaultAsync(b => b.Id == id && b.ProjectId == projectId);
    }

    public Task<bool> ExistsByIdAsync(HashId id, Guid projectId)
    {
        return db.Set<BlobMetadataEntity>()
            .AnyAsync(b => b.Id == id && b.ProjectId == projectId);
    }

    public Task<List<BlobMetadataEntity>> GetAllByProjectIdAsync(Guid projectId)
    {
        return db.Set<BlobMetadataEntity>()
            .Where(b => b.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task AddAsync(BlobMetadataEntity blob)
    {
        await db.Set<BlobMetadataEntity>().AddAsync(blob);
    }

    public void Delete(BlobMetadataEntity blob)
    {
        db.Set<BlobMetadataEntity>().Remove(blob);
    }

    public Task SaveChangesAsync()
    {
        return db.SaveChangesAsync();
    }
}