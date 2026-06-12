using Core.Storage;
using Lib.Infrastructure.Vcs;
using Lib.Modules.Vcs.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Vcs.Repository;

public class SnapshotMetadataRepository(VcsDbContext db) : ISnapshotMetadataRepository
{
    public Task<SnapshotMetadata?> GetAsync(
        HashId commitId,
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        return db.Set<SnapshotMetadata>()
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.CommitId == commitId && m.ProjectId == projectId, cancellationToken);
    }

    public async Task UpsertAsync(
        SnapshotMetadata entity,
        CancellationToken cancellationToken = default
    )
    {
        var existing = await db.Set<SnapshotMetadata>()
            .FirstOrDefaultAsync(
                m => m.CommitId == entity.CommitId && m.ProjectId == entity.ProjectId,
                cancellationToken
            );

        if (existing is null)
        {
            await db.Set<SnapshotMetadata>().AddAsync(entity, cancellationToken);
        }
        else
        {
            db.Entry(existing).CurrentValues.SetValues(entity);
        }
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return db.SaveChangesAsync(cancellationToken);
    }
}