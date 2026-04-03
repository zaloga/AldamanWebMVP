using Aldaman.Persistence.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aldaman.Persistence.Context;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public DbSet<PageDefinitionEntity> PageDefinitions { get; set; } = null!;
    public DbSet<PageContentEntity> PageContents { get; set; } = null!;
    public DbSet<MediaAssetEntity> MediaAssets { get; set; } = null!;
    public DbSet<BlogPostEntity> BlogPosts { get; set; } = null!;
    public DbSet<BlogPostTranslationEntity> BlogPostTranslations { get; set; } = null!;
    public DbSet<ContactMessageEntity> ContactMessages { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all configurations from the current assembly
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global query filters for soft delete
        builder.Entity<BlogPostEntity>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<PageDefinitionEntity>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<MediaAssetEntity>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ContactMessageEntity>().HasQueryFilter(e => !e.IsDeleted);
    }
}
