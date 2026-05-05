namespace Aldaman.Persistence.Entities;

public class BlogPostEntity : BaseEntityAuditableSoftDel
{
    public Guid? CoverMediaAssetId { get; set; }

    public DateTime? PublishedAtUtc { get; set; }

    public bool IsPublished { get; set; }

    // Navigation properties
    public virtual MediaAssetEntity? CoverMediaAsset { get; set; }
    public virtual ICollection<BlogPostTranslationEntity> Translations { get; set; } = new List<BlogPostTranslationEntity>();
}
