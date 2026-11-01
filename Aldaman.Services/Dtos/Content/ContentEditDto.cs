using Aldaman.Persistence.Enums;
using Microsoft.AspNetCore.Http;

namespace Aldaman.Services.Dtos.Content;

/// <summary>
/// DTO for creating or updating a content entity with its localized translations.
/// </summary>
public class ContentEditDto
{
    public Guid? Id { get; set; }
    public ContentTypeEnum ContentType { get; set; } = ContentTypeEnum.Post;
    public PlaceToShowEnum PlaceToShow { get; set; } = PlaceToShowEnum.None;
    public int Order { get; set; } = 0;
    public Guid? CoverMediaAssetId { get; set; }
    public IFormFile? CoverImageFile { get; set; }
    public string? CoverImageRelativePath { get; set; }
    public bool RemoveCoverImage { get; set; }
    public bool IsPublished { get; set; } = true;
    public DateTime? PublishedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }

    public List<ContentTranslationDto> Translations { get; set; } = new();

    /// <summary>
    /// Content group (page) to which this content belongs when created or edited.
    /// </summary>
    public Guid? ContentGroupId { get; set; }

    /// <summary>
    /// List of available content groups (pages) for selection.
    /// </summary>
    public List<Aldaman.Services.Dtos.ContentGroup.ContentGroupItemOptionDto> AvailableContentGroups { get; set; } = new();
}
