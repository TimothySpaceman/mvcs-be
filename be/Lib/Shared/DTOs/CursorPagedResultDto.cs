namespace Lib.Shared.DTOs;

public record CursorPagedResultDto<TItem, TCursor>(
    IEnumerable<TItem> Items,
    int Limit,
    TCursor? NextCursor
);