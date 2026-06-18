using Lib.Infrastructure.App;
using Lib.Modules.Tasks.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lib.Modules.Tasks.Repositories;

public class TaskRepository(AppDbContext db) : ITaskRepository
{
    public Task<ProjectTask?> GetByIdAsync(Guid id)
    {
        return db.Set<ProjectTask>()
            .Include(t => t.Assignments)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public Task<List<ProjectTask>> GetAllAsync(TaskFilter filter)
    {
        var query = db.Set<ProjectTask>()
            .Include(t => t.Assignments)
            .AsQueryable();

        if (filter.ProjectId is not null)
            query = query.Where(t => t.ProjectId == filter.ProjectId);

        if (filter.AssignedUserId is not null)
            query = query.Where(t => t.Assignments.Any(a => a.UserId == filter.AssignedUserId));

        if (filter.Status is not null)
            query = query.Where(t => t.Status == filter.Status);

        return query.ToListAsync();
    }

    public async Task AddAsync(ProjectTask task)
    {
        await db.Set<ProjectTask>().AddAsync(task);
    }

    public void Delete(ProjectTask task)
    {
        db.Set<ProjectTask>().Remove(task);
    }

    public async Task AddAssignmentAsync(TaskAssignment assignment)
    {
        await db.Set<TaskAssignment>().AddAsync(assignment);
    }

    public void DeleteAssignment(TaskAssignment assignment)
    {
        db.Set<TaskAssignment>().Remove(assignment);
    }

    public Task SaveChangesAsync()
    {
        return db.SaveChangesAsync();
    }
}