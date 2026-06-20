using Lib.Modules.Tasks.Entities;

namespace Lib.Modules.Tasks.Repositories;

public interface ITaskRepository
{
    public Task<ProjectTask?> GetByIdAsync(Guid id);
    public Task<List<ProjectTask>> GetAllAsync(TaskFilter filter);
    public Task AddAsync(ProjectTask task);
    public void Delete(ProjectTask task);
    public Task AddAssignmentAsync(TaskAssignment assignment);
    public void DeleteAssignment(TaskAssignment assignment);
    public Task SaveChangesAsync();
}