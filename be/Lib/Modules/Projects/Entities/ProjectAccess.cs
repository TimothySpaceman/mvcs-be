namespace Lib.Modules.Projects.Entities;

public enum ProjectAccessType
{
    ReadOnly = 0,
    ReadWrite = 1
}

public class ProjectAccess
{
    public Guid ProjectId { get; private set; }
    public Guid UserId { get; private set; }
    public ProjectAccessType AccessType { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public Project Project { get; private set; } = null!;

    private ProjectAccess()
    {
    }

    public static ProjectAccess Create(Guid projectId, Guid userId, ProjectAccessType accessType)
    {
        return new ProjectAccess
        {
            ProjectId = projectId,
            UserId = userId,
            AccessType = accessType,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public void ChangeAccessType(ProjectAccessType accessType)
    {
        AccessType = accessType;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public bool CanWrite => AccessType is ProjectAccessType.ReadWrite;
    public bool CanRead => true;
}