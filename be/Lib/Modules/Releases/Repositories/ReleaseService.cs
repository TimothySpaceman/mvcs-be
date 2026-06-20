using Lib.Infrastructure.App;
using Lib.Modules.Releases.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Releases.Repositories;

public class ReleaseRepository(AppDbContext db) : IReleaseRepository
{
    public Task<List<Release>> GetAllAsync(ReleaseFilter filter)
    {
        return BuildQuery(filter)
            .Include(r => r.Files)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((filter.Page - 1) * filter.ItemsPerPage)
            .Take(filter.ItemsPerPage)
            .ToListAsync();
    }

    public Task<int> CountAsync(ReleaseFilter filter)
    {
        return BuildQuery(filter).CountAsync();
    }

    private IQueryable<Release> BuildQuery(ReleaseFilter filter)
    {
        var query = db.Set<Release>().AsQueryable();

        if (filter.ProjectId is not null)
            query = query.Where(r => r.ProjectId == filter.ProjectId);

        return query;
    }

    public Task<Release?> GetLatestByProjectIdAsync(Guid projectId)
    {
        return db.Set<Release>()
            .Include(r => r.Files)
            .Where(r => r.ProjectId == projectId)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public Task<Release?> GetByIdAsync(Guid id)
    {
        return db.Set<Release>()
            .Include(r => r.Files)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public Task<ReleaseFile?> GetFileByIdAsync(Guid id)
    {
        return db.Set<ReleaseFile>()
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task AddAsync(Release release)
    {
        await db.Set<Release>().AddAsync(release);
    }

    public void Delete(Release release)
    {
        db.Set<Release>().Remove(release);
    }

    public Task SaveChangesAsync()
    {
        return db.SaveChangesAsync();
    }
}