using Lib.Infrastructure.App;
using Lib.Modules.Projects.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Projects.Repositories;

public class ProjectRepository(AppDbContext db) : IProjectRepository
{
    public Task<List<Project>> SearchAsync(ProjectFilter filter, Guid? viewerUserId)
    {
        return BuildQuery(filter, viewerUserId)
            .OrderByDescending(p => p.UpdatedAt)
            .Skip((filter.Page - 1) * filter.ItemsPerPage)
            .Take(filter.ItemsPerPage)
            .ToListAsync();
    }

    public Task<int> CountAsync(ProjectFilter filter, Guid? viewerUserId)
    {
        return BuildQuery(filter, viewerUserId).CountAsync();
    }

    private IQueryable<Project> BuildQuery(ProjectFilter filter, Guid? viewerUserId)
    {
        var query = db.Set<Project>()
            .Include(p => p.AccessEntries)
            .AsQueryable();
        

        if (filter.ExplicitAccessOnly is not null && filter.ExplicitAccessOnly.Value && viewerUserId.HasValue)
            query = query.Where(p => p.AuthorId == viewerUserId.Value || p.AccessEntries.Any(a => a.UserId == viewerUserId.Value));
        else if (viewerUserId.HasValue)
            query = query.Where(p => p.IsPublic || p.AuthorId == viewerUserId.Value || p.AccessEntries.Any(a => a.UserId == viewerUserId.Value));
        else
            query = query.Where(p => p.IsPublic);

        if (filter.IsPublic is not null)
            query = query.Where(p => p.IsPublic == filter.IsPublic);

        if (filter.AuthorId is not null)
            query = query.Where(p => p.AuthorId == filter.AuthorId);

        if (filter.StorageId is not null)
            query = query.Where(p => p.StorageId == filter.StorageId);

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(p => p.Title.Contains(filter.Search) || p.Description.Contains(filter.Search));

        return query;
    }
    
    public Task<List<Project>> GetAllByAuthorIdAsync(Guid userId)
    {
        return db.Set<Project>()
            .Include(p => p.AccessEntries)
            .Where(s => s.AuthorId == userId)
            .ToListAsync();
    }

    public Task<List<Project>> GetAllByStorageIdAsync(Guid storageId)
    {
        return db.Set<Project>()
            .Include(p => p.AccessEntries)
            .Where(s => s.StorageId == storageId)
            .ToListAsync();
    }

    public Task<Project?> GetByIdAsync(Guid id)
    {
        return db.Set<Project>()
            .Include(p => p.AccessEntries)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public Task<bool> ExistsByIdAsync(Guid id)
    {
        return db.Set<Project>().AnyAsync(s => s.Id == id);
    }

    public async Task AddAsync(Project project)
    {
        await db.Set<Project>().AddAsync(project);
    }

    public void Delete(Project project)
    {
        db.Set<Project>().Remove(project);
    }

    public async Task AddAccessAsync(ProjectAccess access)
    {
        await db.Set<ProjectAccess>().AddAsync(access);
    }

    public void DeleteAccess(ProjectAccess access)
    {
        db.Set<ProjectAccess>().Remove(access);
    }

    public Task SaveChangesAsync()
    {
        return db.SaveChangesAsync();
    }
}