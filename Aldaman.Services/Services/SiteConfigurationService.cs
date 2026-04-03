using Aldaman.Services.Dtos.SiteConfiguration;
using Aldaman.Services.Interfaces;

namespace Aldaman.Services.Services;

public sealed class SiteConfigurationService : ISiteConfigurationService
{
    public Task<SiteConfigurationDto> GetConfigurationAsync() => throw new NotImplementedException();
    public Task UpdateConfigurationAsync(SiteConfigurationDto dto) => throw new NotImplementedException();
}
