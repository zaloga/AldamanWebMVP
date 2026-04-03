using System.Collections.Generic;

namespace Aldaman.Services.Dtos.General;

/// <summary>
/// Generic paged result wrapper.
/// </summary>
public class PagedResultDto<T> : PagedResultBase
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
}
