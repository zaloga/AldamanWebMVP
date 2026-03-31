namespace Aldaman.Persistence.Entities;

public class MediaAssetEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string OriginalFileName { get; set; } = string.Empty;

    public string StoredFileName { get; set; } = string.Empty;

    public string RelativePath { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public string? AltText { get; set; }

    public string? Title { get; set; }

    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;

    public Guid UploadedByUserId { get; set; }

    public bool IsImage { get; set; }

    public bool IsVideo { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation property
    public virtual AppUser UploadedByUser { get; set; } = null!;
}
