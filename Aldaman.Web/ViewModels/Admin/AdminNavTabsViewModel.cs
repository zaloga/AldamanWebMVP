namespace Aldaman.Web.ViewModels.Admin;

public sealed class AdminNavTabsViewModel
{
    public required bool IsDeleted { get; init; }
    public required string ActiveTitle { get; init; }
    public required string DeletedTitle { get; init; }
    public string ActiveIcon { get; init; } = "bi-collection";
    public string DeletedIcon { get; init; } = "bi-trash";
    public int? ActiveCount { get; init; }
    public int? DeletedCount { get; init; }
}
