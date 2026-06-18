using Lib.Modules.Tasks.Entities;

namespace Lib.Modules.Tasks.Repositories;

public interface ITaskRepository
{
    Task<ProjectTask?> GetByIdAsync(Guid id);
    Task<List<ProjectTask>> GetAllAsync(TaskFilter filter);
    Task AddAsync(ProjectTask task);
    void Delete(ProjectTask task);
    Task AddAssignmentAsync(TaskAssignment assignment);
    void DeleteAssignment(TaskAssignment assignment);
    Task SaveChangesAsync();
}