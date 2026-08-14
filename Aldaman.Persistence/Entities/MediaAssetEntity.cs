namespace Aldaman.Persistence.Entities;

public class MediaAssetEntity : BaseEntityAuditableSoftDel
{
    public const int TitleDefaultMaxLength = 512;
    public const int TitleDefaultMinLength = 3;
    public const int AltTextDefaultMaxLength = 512;
    public const int AltTextDefaultMinLength = 3;
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

    public string? AltTextDefault { get; set; }

    public string? TitleDefault { get; set; }

    public bool IsImage { get; set; }

    public bool IsVideo { get; set; }
}
