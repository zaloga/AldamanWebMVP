namespace Aldaman.Persistence.Entities;

public class BlogPostEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? CoverMediaAssetId { get; set; }

    public DateTime? PublishedAtUtc { get; set; }

    public bool IsPublished { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Guid CreatedByUserId { get; set; }

    public Guid UpdatedByUserId { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedByUserId { get; set; }

    // Navigation properties
    public virtual MediaAssetEntity? CoverMediaAsset { get; set; }
    public virtual AppUser CreatedByUser { get; set; } = null!;
    public virtual AppUser UpdatedByUser { get; set; } = null!;
    public virtual AppUser? DeletedByUser { get; set; }
    public virtual ICollection<BlogPostTranslationEntity> Translations { get; set; } = new List<BlogPostTranslationEntity>();
}
