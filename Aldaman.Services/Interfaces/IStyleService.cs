using Aldaman.Services.Dtos.StyleSettings;

namespace Aldaman.Services.Interfaces;

public interface IStyleService
{
    Task<Dictionary<string, string>> GetActiveStylesAsync(CancellationToken ct = default);
    Task<List<StyleSettingDto>> GetAllSettingsAsync(CancellationToken ct = default);
    Task<StyleSettingDto?> GetSettingByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateSettingAsync(UpdateStyleSettingDto dto, CancellationToken ct = default);
    Task ResetToDefaultSettingAsync(Guid id, CancellationToken ct = default);
    Task SoftDeleteSettingAsync(Guid id, CancellationToken ct = default);
    Task RestoreSettingAsync(Guid id, CancellationToken ct = default);
    Task HardDeleteSettingAsync(Guid id, CancellationToken ct = default);
    Task<List<StyleSettingDto>> GetDeletedSettingsAsync(CancellationToken ct = default);
}
