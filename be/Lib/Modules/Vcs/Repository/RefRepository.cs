using Lib.Infrastructure.Vcs;
using Lib.Modules.Vcs.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Vcs.Repository;

public class RefRepository(VcsDbContext db) : IRefRepository
{
    public Task<RefEntity?> GetAsync(Guid projectId, string name, CancellationToken cancellationToken = default)
    {
        return db.Set<RefEntity>()
            .FirstOrDefaultAsync(r => r.ProjectId == projectId && r.Name == name, cancellationToken);
    }

    public Task<RefEntity?> GetForUpdateAsync(Guid projectId, string name,
        CancellationToken cancellationToken = default)
    {
        return db.Set<RefEntity>()
            .FromSqlInterpolated(
                $"""
                 SELECT * FROM "refs"
                 WHERE "ProjectId" = {projectId} AND "Name" = {name}
                 FOR UPDATE
                 """
            )
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<RefEntity>> GetAllAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return db.Set<RefEntity>()
            .Where(r => r.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(RefEntity entry, CancellationToken cancellationToken = default)
    {
        await db.Set<RefEntity>().AddAsync(entry, cancellationToken);
    }

    public void Delete(RefEntity entry)
    {
        db.Set<RefEntity>().Remove(entry);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return db.SaveChangesAsync(cancellationToken);
    }
}