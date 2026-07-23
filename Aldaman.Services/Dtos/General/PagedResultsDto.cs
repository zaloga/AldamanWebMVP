namespace Aldaman.Services.Dtos.General;

/// <summary>
/// Wrapper for both active and deleted paginated results.
/// </summary>
public class PagedResultsDto<T>
{
    public PaginationQuery Query { get; set; } = new();
    public PagedResultDto<T> Items { get; set; } = new();
    public PaginationQuery DeletedQuery { get; set; } = new();
    public PagedResultDto<T>? DeletedItems { get; set; }
}
