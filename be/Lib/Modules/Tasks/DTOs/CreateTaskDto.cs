namespace Lib.Modules.Tasks.DTOs;

public record CreateTaskDto(
    string Title,
    string? Description,
    DateTimeOffset? Deadline,
    List<Guid> AssignedUserIds
);