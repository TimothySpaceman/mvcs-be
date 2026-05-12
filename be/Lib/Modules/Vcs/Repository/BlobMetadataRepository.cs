using Core.Blobs;
using Core.Storage;
using Lib.Infrastructure.Vcs;
using Lib.Modules.Vcs.Entities;
using Lib.Modules.Vcs.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Vcs.Repository;

public class BlobMetadataRepository(VcsDbContext db) : IBlobMetadataRepository
{
    public async Task<BlobMetadata?> GetByIdAsync(
        HashId id,
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await db.Set<BlobMetadataEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id && b.ProjectId == projectId, cancellationToken);

        return entity?.ToDomain();
    }

    public Task<bool> ExistsByIdAsync(
        HashId id,
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        return db.Set<BlobMetadataEntity>()
            .AnyAsync(b => b.Id == id && b.ProjectId == projectId, cancellationToken);
    }

    public async Task<bool> AllExistAsync(
        IEnumerable<HashId> ids,
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        var idList = ids.ToList();
        var foundCount = await db.Set<BlobMetadataEntity>()
            .Where(b => b.ProjectId == projectId && idList.Contains(b.Id))
            .CountAsync(cancellationToken);

        return foundCount == idList.Count;
    }

    public async Task<List<BlobMetadata>> GetAllByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        return await db.Set<BlobMetadataEntity>()
            .AsNoTracking()
            .Where(b => b.ProjectId == projectId)
            .Select(b => b.ToDomain())
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        BlobMetadata blob,
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        var entity = blob.ToEntity(projectId);
        await db.Set<BlobMetadataEntity>().AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(
        HashId id,
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        await db.Set<BlobMetadataEntity>()
            .Where(b => b.Id == id && b.ProjectId == projectId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        return db.SaveChangesAsync(cancellationToken);
    }
}