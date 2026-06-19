namespace Lib.Modules.Releases.Entities;

public class Release
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid? AuthorId { get; private set; }
    public string Title { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<ReleaseFile> Files => _files.AsReadOnly();
    private readonly List<ReleaseFile> _files = [];
    
    private Release()
    {
    }

    public static Release Create(
        Guid projectId,
        Guid authorId,
        string title
        )
    {
        return new Release
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            AuthorId = authorId,
            Title = title,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}