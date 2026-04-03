namespace Aldaman.Persistence.Entities;

public class MediaAssetEntity : BaseEntity
{
    public string OriginalFileName { get; set; } = string.Empty;

    public string StoredFileName { get; set; } = string.Empty;

    public string RelativePath { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public string? AltText { get; set; }

    public string? Title { get; set; }
    
    // We keep UploadedAtUtc/UploadedByUserId as duplicates of CreatedAtUtc/CreatedByUserId? No, let's keep them if mapping requires or rename to use BaseEntity properties. 
    // Since these were explicit, let's just drop them and use CreatedAtUtc and CreatedByUserId instead of UploadedAtUtc and UploadedByUserId.
    // However, since we'll drop them entirely, I will just remove them from this class. 
    // The Base properties have everything we need!

    public bool IsImage { get; set; }

    public bool IsVideo { get; set; }
}
