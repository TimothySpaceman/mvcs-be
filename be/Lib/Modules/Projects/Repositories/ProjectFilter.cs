namespace Lib.Modules.Projects.Repositories;

public class ProjectFilter
{
    public int Page { get; init; } = 1;
    public int ItemsPerPage { get; init; } = 20;
    public bool? IsPublic { get; init; }
    public string? Search { get; init; }
    public Guid? AuthorId { get; init; }
    public Guid? StorageId { get; init; }
    public bool? ExplicitAccessOnly { get; init; } = false;
}