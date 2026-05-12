using Aldaman.Services.Dtos.StyleSettings;

namespace Aldaman.Services.Interfaces;

public interface IStyleService
{
    Task<Dictionary<string, string>> GetActiveStylesAsync();
    Task<List<StyleSettingDto>> GetAllSettingsAsync();
    Task<StyleSettingDto?> GetSettingByIdAsync(Guid id);
    Task UpdateSettingAsync(UpdateStyleSettingDto dto);
    Task ResetToDefaultSettingAsync(Guid id);
    Task SoftDeleteSettingAsync(Guid id);
    Task RestoreSettingAsync(Guid id);
    Task HardDeleteSettingAsync(Guid id);
    Task<List<StyleSettingDto>> GetDeletedSettingsAsync();
}
