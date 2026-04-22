namespace Aldaman.Persistence.Entities;

public class MediaAssetEntity : BaseEntity
{
    public const int TitleMaxLength = 512;
    public const int TitleMinLength = 3;
    public const int AltTextMaxLength = 512;
    public const int AltTextMinLength = 3;
    public const int OriginalFileNameMaxLength = 256;
    public const int StoredFileNameMaxLength = 256;
    public const int RelativePathMaxLength = 512;
    public const int ContentTypeMaxLength = 128;

    public string OriginalFileName { get; set; } = string.Empty;

    public string StoredFileName { get; set; } = string.Empty;

    public string RelativePath { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public string? AltText { get; set; }

    public string? Title { get; set; }

    public bool IsImage { get; set; }

    public bool IsVideo { get; set; }
}
