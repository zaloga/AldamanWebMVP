namespace Aldaman.Services.Dtos.Media;

public class UpdateMediaAssetDto
{
    public Guid Id { get; set; }
    public string? AltText { get; set; }
    public string? Title { get; set; }
}
