using Lib.Modules.Tasks.DTOs;
using Lib.Modules.Tasks.Entities;
using Lib.Modules.Tasks.Repositories;
using Lib.Shared.Exceptions;

namespace Lib.Modules.Tasks.Services;

public class TaskService(ITaskRepository repository) : ITaskService
{
    public async Task<List<TaskDto>> GetAllAsync(TaskFilter filter)
    {
        var tasks = await repository.GetAllAsync(filter);
        return tasks.Select(TaskDto.FromEntity).ToList();
    }

    public async Task<TaskDto> GetByIdAsync(Guid id)
    {
        var task = await GetRawByIdAsync(id);
        return TaskDto.FromEntity(task);
    }

    public async Task<ProjectTask> GetRawByIdAsync(Guid id)
    {
        var task = await repository.GetByIdAsync(id);
        if (task is null) throw new NotFoundException("Task not found");
        return task;
    }

    public async Task<TaskDto> CreateAsync(Guid projectId, Guid authorId, CreateTaskDto dto)
    {
        var task = ProjectTask.Create(projectId, authorId, dto.Title, dto.Description, dto.Deadline);

        await repository.AddAsync(task);

        foreach (var userId in dto.AssignedUserIds)
        {
            await repository.AddAssignmentAsync(TaskAssignment.Create(task.Id, userId));
        }

        await repository.SaveChangesAsync();
        return TaskDto.FromEntity(task);
    }

    public async Task<TaskDto> UpdateAsync(Guid id, UpdateTaskDto dto)
    {
        var task = await GetRawByIdAsync(id);
        task.Update(dto.Title, dto.Description, dto.Deadline);

        if (task.CommitId != dto.CommitId)
        {
            task.UpdateCommit(dto.CommitId);
            if (dto.CommitId is not null && task.Status == Entities.TaskStatus.InProgress)
            {
                task.ChangeStatus(Entities.TaskStatus.Review);
            }
        }

        if (dto.Status is not null && task.Status != dto.Status) task.ChangeStatus(dto.Status.Value);

        await repository.SaveChangesAsync();
        return TaskDto.FromEntity(task);
    }

    public async Task AssignUserAsync(Guid id, Guid userId)
    {
        var task = await GetRawByIdAsync(id);
        if (task.Assignments.Any(a => a.UserId == userId)) return;
        await repository.AddAssignmentAsync(TaskAssignment.Create(task.Id, userId));
        await repository.SaveChangesAsync();
    }

    public async Task UnassignUserAsync(Guid id, Guid userId)
    {
        var task = await GetRawByIdAsync(id);
        var assignment = task.Assignments.FirstOrDefault(a => a.UserId == userId);
        if (assignment is null) throw new NotFoundException("Assignment not found");
        repository.DeleteAssignment(assignment);
        await repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var task = await GetRawByIdAsync(id);
        repository.Delete(task);
        await repository.SaveChangesAsync();
    }
}