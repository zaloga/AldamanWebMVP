using Aldaman.Persistence.Enums;

namespace Aldaman.Persistence.Entities;

public sealed class StyleSettingEntity : BaseEntityAuditableSoftDel
{
    public required CssType Type { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }
    public required string DefaultValue { get; set; }
}
