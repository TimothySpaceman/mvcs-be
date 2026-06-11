using Lib.Infrastructure.Vcs;
using Lib.Modules.Vcs.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Vcs.Repository;

public class MergeRequestRepository(VcsDbContext db) : IMergeRequestRepository
{
    public Task<MergeRequest?> GetAsync(Guid projectId, Guid id, CancellationToken cancellationToken = default)
    {
        return db.Set<MergeRequest>()
            .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.Id == id, cancellationToken);
    }

    public Task<List<MergeRequest>> GetAllByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default
    )
    {
        return db.Set<MergeRequest>()
            .Where(m => m.ProjectId == projectId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MergeRequest mergeRequest, CancellationToken cancellationToken = default)
    {
        await db.Set<MergeRequest>().AddAsync(mergeRequest, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return db.SaveChangesAsync(cancellationToken);
    }
}