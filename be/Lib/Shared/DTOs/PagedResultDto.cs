namespace Lib.Shared.DTOs;

public record PagedResultDto<T>(
    IEnumerable<T> Items,
    int Page,
    int ItemsPerPage,
    int TotalItems
);