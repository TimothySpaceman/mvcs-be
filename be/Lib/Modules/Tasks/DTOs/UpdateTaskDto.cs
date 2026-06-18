namespace Lib.Modules.Tasks.DTOs;

public record UpdateTaskDto(
    string? Title,
    string? Description,
    DateTimeOffset? Deadline,
    string? CommitId,
    Entities.TaskStatus? Status
);