namespace Lib.Modules.Tasks.Repositories;

public class TaskFilter
{
    public Guid? ProjectId { get; init; }
    public Guid? AssignedUserId { get; init; }
    public Entities.TaskStatus? Status { get; init; }
}