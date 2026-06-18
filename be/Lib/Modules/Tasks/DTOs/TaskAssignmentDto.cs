using Lib.Modules.Tasks.Entities;

namespace Lib.Modules.Tasks.DTOs;

public record TaskAssignmentDto(Guid UserId)
{
    public static TaskAssignmentDto FromEntity(TaskAssignment entity) => new(entity.UserId);
}