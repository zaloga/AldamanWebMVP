using Aldaman.Persistence.Enums;

namespace Aldaman.Persistence.Entities;

/// <summary>
/// Unified entity for pages and articles.
/// </summary>
public class ContentEntity : BaseEntityAuditableSoftDel
{
    /// <summary>
    /// Content display type (e.g. Page, Post).
    /// </summary>
    public ContentTypeEnum ContentType { get; set; } = ContentTypeEnum.Post;

    /// <summary>
    /// Indicates where this content item should be displayed (e.g. HomePage, TopNavigation, Footer).
    /// </summary>
    public PlaceToShowEnum PlaceToShow { get; set; } = PlaceToShowEnum.None;

    /// <summary>
    /// Sorting order of the content item.
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// Optional cover image media asset ID.
    /// </summary>
    public Guid? CoverMediaAssetId { get; set; }

    /// <summary>
    /// Publication timestamp in UTC.
    /// </summary>
    public DateTime? PublishedAtUtc { get; set; }

    /// <summary>
    /// Indicates whether the content item is published.
    /// </summary>
    public bool IsPublished { get; set; } = true;

    // Navigation properties
    public virtual MediaAssetEntity? CoverMediaAsset { get; set; }
    public virtual ICollection<ContentTranslationEntity> Translations { get; set; } = new List<ContentTranslationEntity>();
    public virtual ICollection<ContentGroupContentEntity> ContentGroups { get; set; } = new List<ContentGroupContentEntity>();
}
