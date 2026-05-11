using Core.Commits;
using Core.Storage;
using Lib.Infrastructure.Vcs;
using Lib.Modules.Vcs.Entities;
using Lib.Modules.Vcs.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Vcs.Repository;

public class CommitRepository(VcsDbContext db, Guid projectId) : ICommitStore
{
    public Task<bool> HasAsync(HashId id, CancellationToken cancellationToken = default)
    {
        return db.Set<CommitEntity>()
            .AnyAsync(c => c.Id == id && c.ProjectId == projectId, cancellationToken);
    }

    public async Task<Commit?> GetAsync(HashId id, CancellationToken cancellationToken = default)
    {
        var entity = await db.Set<CommitEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.ProjectId == projectId, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<Dictionary<HashId, Commit>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await db.Set<CommitEntity>()
            .AsNoTracking()
            .Where(c => c.ProjectId == projectId)
            .ToDictionaryAsync(c => c.Id, c => c.ToDomain(), cancellationToken);
    }

    public async Task AddAsync(Commit commit, CancellationToken cancellationToken = default)
    {
        var exists = await db.Set<CommitEntity>()
            .AnyAsync(c => c.Id == commit.Id && c.ProjectId == projectId, cancellationToken);

        if (exists) return;

        var entity = commit.ToEntity(projectId);
        await db.Set<CommitEntity>().AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RemoveAsync(HashId id, CancellationToken cancellationToken = default)
    {
        var deleted = await db.Set<CommitEntity>()
            .Where(c => c.Id == id && c.ProjectId == projectId)
            .ExecuteDeleteAsync(cancellationToken);

        return deleted > 0;
    }
}