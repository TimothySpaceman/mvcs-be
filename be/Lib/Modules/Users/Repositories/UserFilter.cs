namespace Lib.Modules.Users.Repositories;

public class UserFilter
{
    public int Page { get; init; } = 1;
    public int ItemsPerPage { get; init; } = 20;
    public string? Search { get; init; }
}