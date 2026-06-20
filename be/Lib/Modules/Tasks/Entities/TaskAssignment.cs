namespace Lib.Modules.Tasks.Entities;

public class TaskAssignment
{
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public ProjectTask Task { get; private set; } = null!;

    private TaskAssignment()
    {
    }

    public static TaskAssignment Create(Guid taskId, Guid userId)
    {
        return new TaskAssignment
        {
            TaskId = taskId,
            UserId = userId,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}