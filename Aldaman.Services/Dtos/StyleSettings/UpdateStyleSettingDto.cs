using Aldaman.Persistence.Enums;

namespace Aldaman.Services.Dtos.StyleSettings;

public sealed class UpdateStyleSettingDto
{
    public Guid? Id { get; set; }
    public CssType Type { get; set; }
    public string? Key { get; set; }
    public required string Value { get; set; }
    public string? DefaultValue { get; set; }
}
