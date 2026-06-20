using Lib.Modules.Tasks.DTOs;
using Lib.Modules.Tasks.Entities;
using Lib.Modules.Tasks.Repositories;

namespace Lib.Modules.Tasks.Services;

public interface ITaskService
{
    public Task<List<TaskDto>> GetAllAsync(TaskFilter filter);
    public Task<TaskDto> GetByIdAsync(Guid id);
    public Task<ProjectTask> GetRawByIdAsync(Guid id);
    public Task<TaskDto> CreateAsync(Guid projectId, Guid authorId, CreateTaskDto dto);
    public Task<TaskDto> UpdateAsync(Guid id, UpdateTaskDto dto);
    public Task AssignUserAsync(Guid id, Guid userId);
    public Task UnassignUserAsync(Guid id, Guid userId);
    public Task DeleteAsync(Guid id);
}