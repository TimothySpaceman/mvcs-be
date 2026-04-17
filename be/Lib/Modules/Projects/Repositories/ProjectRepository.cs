using Lib.Infrastructure.App;
using Lib.Modules.Projects.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Projects.Repositories;

public class ProjectRepository(AppDbContext db) : IProjectRepository
{
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