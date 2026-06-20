using Lib.Modules.Tasks.Entities;

namespace Lib.Modules.Tasks.DTOs;

public record TaskDto(
    Guid Id,
    Guid ProjectId,
    Guid? AuthorId,
    string Title,
    string? Description,
    Entities.TaskStatus Status,
    string? CommitId,
    DateTimeOffset? Deadline,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    List<TaskAssignmentDto> Assignments
)
{
    public static TaskDto FromEntity(ProjectTask entity) => new(
        entity.Id,
        entity.ProjectId,
        entity.AuthorId,
        entity.Title,
        entity.Description,
        entity.Status,
        entity.CommitId,
        entity.Deadline,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.Assignments.Select(TaskAssignmentDto.FromEntity).ToList()
    );
}