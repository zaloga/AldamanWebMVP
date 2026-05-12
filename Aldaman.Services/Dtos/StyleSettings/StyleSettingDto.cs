using Aldaman.Persistence.Enums;

namespace Aldaman.Services.Dtos.StyleSettings;

public sealed class StyleSettingDto
{
    public Guid Id { get; set; }
    public CssType Type { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string DefaultValue { get; set; } = string.Empty;
}
