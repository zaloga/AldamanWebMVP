using Aldaman.Persistence.Enums;

namespace Aldaman.Persistence.Entities;

/// <summary>
/// Grouping entity for content items.
/// </summary>
public class ContentGroupEntity : BaseEntityAuditableSoftDel
{
    /// <summary>
    /// Indicates where this group should be displayed.
    /// </summary>
    public PlaceToShowEnum PlaceToShow { get; set; }

    /// <summary>
    /// Sorting order of the group.
    /// </summary>
    public int GroupOrder { get; set; }

    // Navigation properties
    public virtual ICollection<ContentGroupTranslationEntity> Translations { get; set; } = new List<ContentGroupTranslationEntity>();
    public virtual ICollection<ContentGroupContentEntity> Contents { get; set; } = new List<ContentGroupContentEntity>();
}
