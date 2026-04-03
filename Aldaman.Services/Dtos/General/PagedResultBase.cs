using System;

namespace Aldaman.Services.Dtos.General;

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
