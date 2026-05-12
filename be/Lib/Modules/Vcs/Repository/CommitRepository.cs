using Core.Commits;
using Core.Storage;
using Lib.Infrastructure.Vcs;
using Lib.Modules.Vcs.Entities;
using Lib.Modules.Vcs.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Vcs.Repository;

public class CommitRepository(VcsDbContext db) : ICommitRepository
{
    public Task<bool> HasAsync(
        Guid projectId,
        HashId id,
        CancellationToken cancellationToken = default
    )
    {
        return db.Set<CommitEntity>()
            .AnyAsync(c => c.Id == id && c.ProjectId == projectId, cancellationToken);
    }

    public async Task<Commit?> GetAsync(
        Guid projectId,
        HashId id,
        CancellationToken cancellationToken = default
    )
    {
        var entity = await db.Set<CommitEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.ProjectId == projectId, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<Dictionary<HashId, Commit>> GetAllAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        return await db.Set<CommitEntity>()
            .AsNoTracking()
            .Where(c => c.ProjectId == projectId)
            .ToDictionaryAsync(c => c.Id, c => c.ToDomain(), cancellationToken);
    }

    public async Task AddAsync(
        Guid projectId,
        Commit commit,
        CancellationToken cancellationToken = default
    )
    {
        var exists = await db.Set<CommitEntity>()
            .AnyAsync(c => c.Id == commit.Id && c.ProjectId == projectId, cancellationToken);

        if (exists) return;

        var entity = commit.ToEntity(projectId);
        await db.Set<CommitEntity>().AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(
        Guid projectId,
        IEnumerable<Commit> commits,
        CancellationToken cancellationToken = default
    )
    {
        var entities = commits.Select(c => c.ToEntity(projectId)).ToList();
        await db.Set<CommitEntity>().AddRangeAsync(entities, cancellationToken);
    }

    public async Task<bool> RemoveAsync(
        Guid projectId,
        HashId id,
        CancellationToken cancellationToken = default
    )
    {
        var deleted = await db.Set<CommitEntity>()
            .Where(c => c.Id == id && c.ProjectId == projectId)
            .ExecuteDeleteAsync(cancellationToken);

        return deleted > 0;
    }
    
    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        return db.SaveChangesAsync(cancellationToken);
    }
}