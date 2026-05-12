using Aldaman.Persistence.Context;
using Aldaman.Services.Dtos.StyleSettings;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Aldaman.Services.Services;

public sealed class StyleService : IStyleService
{
    private const string CacheKey = "ActiveStyles";
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;

    public StyleService(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Dictionary<string, string>> GetActiveStylesAsync()
    {
        if (!_cache.TryGetValue(CacheKey, out Dictionary<string, string>? styles) || styles == null)
        {
            styles = await _context.StyleSettings
                .AsNoTracking()
                .ToDictionaryAsync(s => s.Key, s => s.Value);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(24));

            _cache.Set(CacheKey, styles, cacheEntryOptions);
        }

        return styles;
    }

    public async Task<List<StyleSettingDto>> GetAllSettingsAsync()
    {
        return await _context.StyleSettings
            .AsNoTracking()
            .Select(s => new StyleSettingDto
            {
                Id = s.Id,
                Type = s.Type,
                Key = s.Key,
                Value = s.Value,
                DefaultValue = s.DefaultValue
            })
            .ToListAsync();
    }

    public async Task<StyleSettingDto?> GetSettingByIdAsync(Guid id)
    {
        return await _context.StyleSettings
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new StyleSettingDto
            {
                Id = s.Id,
                Type = s.Type,
                Key = s.Key,
                Value = s.Value,
                DefaultValue = s.DefaultValue
            })
            .FirstOrDefaultAsync();
    }

    public async Task UpdateSettingAsync(UpdateStyleSettingDto dto)
    {
        Aldaman.Persistence.Entities.StyleSettingEntity? entity;

        if (dto.Id.HasValue && dto.Id.Value != Guid.Empty)
        {
            entity = await _context.StyleSettings.FindAsync(dto.Id.Value);
            if (entity == null) return;

            // Only update fields that are provided (for inline updates)
            entity.Key = dto.Key ?? entity.Key;
            entity.Type = dto.Type != 0 ? dto.Type : entity.Type;
            entity.Value = dto.Value;
            entity.DefaultValue = dto.DefaultValue ?? entity.DefaultValue;
        }
        else
        {
            entity = new Aldaman.Persistence.Entities.StyleSettingEntity
            {
                Type = dto.Type,
                Key = dto.Key ?? string.Empty,
                Value = dto.Value,
                DefaultValue = dto.DefaultValue ?? dto.Value
            };
            _context.StyleSettings.Add(entity);
        }

        await _context.SaveChangesAsync();

        // Invalidate cache
        _cache.Remove(CacheKey);
    }

    public async Task ResetToDefaultSettingAsync(Guid id)
    {
        var entity = await _context.StyleSettings.FindAsync(id);
        if (entity != null)
        {
            entity.Value = entity.DefaultValue;
            await _context.SaveChangesAsync();
            _cache.Remove(CacheKey);
        }
    }

    public async Task SoftDeleteSettingAsync(Guid id)
    {
        var entity = await _context.StyleSettings.FindAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            await _context.SaveChangesAsync();
            _cache.Remove(CacheKey);
        }
    }

    public async Task RestoreSettingAsync(Guid id)
    {
        var entity = await _context.StyleSettings
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (entity != null)
        {
            entity.IsDeleted = false;
            await _context.SaveChangesAsync();
            _cache.Remove(CacheKey);
        }
    }

    public async Task HardDeleteSettingAsync(Guid id)
    {
        var entity = await _context.StyleSettings
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (entity != null)
        {
            _context.StyleSettings.Remove(entity);
            await _context.SaveChangesAsync();
            _cache.Remove(CacheKey);
        }
    }

    public async Task<List<StyleSettingDto>> GetDeletedSettingsAsync()
    {
        return await _context.StyleSettings
            .IgnoreQueryFilters()
            .Where(s => s.IsDeleted)
            .AsNoTracking()
            .Select(s => new StyleSettingDto
            {
                Id = s.Id,
                Type = s.Type,
                Key = s.Key,
                Value = s.Value,
                DefaultValue = s.DefaultValue
            })
            .ToListAsync();
    }
}
