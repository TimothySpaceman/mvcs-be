using Lib.Modules.Tasks.DTOs;
using Lib.Modules.Tasks.Entities;
using Lib.Modules.Tasks.Repositories;

namespace Lib.Modules.Tasks.Services;

public interface ITaskService
{
    Task<List<TaskDto>> GetAllAsync(TaskFilter filter);
    Task<TaskDto> GetByIdAsync(Guid id);
    Task<ProjectTask> GetRawByIdAsync(Guid id);
    Task<TaskDto> CreateAsync(Guid projectId, Guid authorId, CreateTaskDto dto);
    Task<TaskDto> UpdateAsync(Guid id, UpdateTaskDto dto);
    Task AssignUserAsync(Guid id, Guid userId);
    Task UnassignUserAsync(Guid id, Guid userId);
    Task DeleteAsync(Guid id);
}