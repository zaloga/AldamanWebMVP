namespace Aldaman.Services.Dtos.Media;

public class UpdateMediaAssetDto
{
    public Guid Id { get; set; }
    public string? AltTextDefault { get; set; }
    public string? TitleDefault { get; set; }
}
