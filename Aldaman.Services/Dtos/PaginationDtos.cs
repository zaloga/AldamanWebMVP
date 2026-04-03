namespace Aldaman.Services.Dtos;

/// <summary>
/// Base class for pagination, sorting and filtering.
/// </summary>
public class PaginationQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public string? SearchTerm { get; set; }
}

/// <summary>
/// Non-generic base class for paged results, suitable for pagination views.
/// </summary>
public class PagedResultBase
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

/// <summary>
/// Generic paged result wrapper.
/// </summary>
public class PagedResultDto<T> : PagedResultBase
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
}

