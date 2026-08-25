using Aldaman.Persistence.Enums;

namespace Aldaman.Services.Dtos.ContentGroup;

/// <summary>
/// DTO for creating or updating a content group with its localized content.
/// </summary>
public class ContentGroupEditDto
{
    public Guid? Id { get; set; }

    public PlaceToShowEnum PlaceToShow { get; set; } = PlaceToShowEnum.None;
    public int GroupOrder { get; set; } = 0;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }

    public List<ContentGroupTranslationDto> Translations { get; set; } = new();

    public List<ContentGroupItemSelectionDto> SelectedContentPages { get; set; } = new();
    public List<ContentGroupItemSelectionDto> SelectedBlogPosts { get; set; } = new();

    public List<ContentGroupItemOptionDto> AvailableContentPages { get; set; } = new();
    public List<ContentGroupItemOptionDto> AvailableBlogPosts { get; set; } = new();
}

