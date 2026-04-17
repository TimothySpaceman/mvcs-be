namespace Lib.Modules.Projects.Entities;

public class Project
{
    public Guid Id { get; private set; }
    public Guid AuthorId { get; private set; }
    public Guid StorageId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public bool IsPublic { get; private set; }
    public bool IsInitialized { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public IReadOnlyCollection<ProjectAccess> AccessEntries => _accessEntries.AsReadOnly();
    private readonly List<ProjectAccess> _accessEntries = [];

    private Project()
    {
    }

    public static Project Create(Guid authorId, Guid storageId, string title, string description, bool isPublic)
    {
        return new Project
        {
            Id = Guid.NewGuid(),
            AuthorId = authorId,
            StorageId = storageId,
            Title = title,
            Description = description,
            IsPublic = isPublic,
            IsInitialized = false,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public bool IsDeleted => DeletedAt.HasValue;

    public void Initialize()
    {
        IsInitialized = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Rename(string title)
    {
        Title = title;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateVisibility(bool isPublic)
    {
        IsPublic = isPublic;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Delete()
    {
        DeletedAt = DateTimeOffset.UtcNow;
    }
}