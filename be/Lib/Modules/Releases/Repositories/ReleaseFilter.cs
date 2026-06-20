namespace Lib.Modules.Releases.Repositories;

public class ReleaseFilter
{
    public int Page { get; init; } = 1;
    public int ItemsPerPage { get; init; } = 20;
    public Guid? ProjectId { get; init; }
}