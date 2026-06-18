namespace Lib.Modules.Tasks.Entities;

public enum TaskStatus
{
    ToDo = 0,
    InProgress = 1,
    Review = 2,
    Done = 3
}

public class ProjectTask
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public string? CommitId { get; private set; }
    public DateTimeOffset? Deadline { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyCollection<TaskAssignment> Assignments => _assignments.AsReadOnly();
    private readonly List<TaskAssignment> _assignments = [];

    private ProjectTask()
    {
    }

    public static ProjectTask Create(
        Guid projectId,
        Guid authorId,
        string title,
        string? description,
        DateTimeOffset? deadline
    )
    {
        return new ProjectTask
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            AuthorId = authorId,
            Title = title,
            Description = description,
            Status = TaskStatus.ToDo,
            CommitId = null,
            Deadline = deadline,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Update(string? title, string? description, DateTimeOffset? deadline)
    {
        Title = title ?? Title;
        Description = description ?? Description;
        Deadline = deadline ?? Deadline;

        if (title is not null || description is not null || deadline is not null)
        {
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }

    public void ChangeStatus(TaskStatus status)
    {
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateCommit(string? commitId)
    {
        CommitId = commitId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}