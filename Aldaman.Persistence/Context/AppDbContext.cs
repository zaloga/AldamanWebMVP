using Aldaman.Persistence.Entities;
using Aldaman.Persistence.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aldaman.Persistence.Context;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public DbSet<ContentPageEntity> ContentPages { get; set; } = null!;
    public DbSet<ContentPageTranslationEntity> ContentPageTranslations { get; set; } = null!;
    public DbSet<MediaAssetEntity> MediaAssets { get; set; } = null!;
    public DbSet<BlogPostEntity> BlogPosts { get; set; } = null!;
    public DbSet<BlogPostTranslationEntity> BlogPostTranslations { get; set; } = null!;
    public DbSet<ContactMessageEntity> ContactMessages { get; set; } = null!;

    private IUserContext UserContext { get; }

    public AppDbContext(DbContextOptions<AppDbContext> options, IUserContext userContext)
        : base(options)
    {
        UserContext = userContext;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all configurations from the current assembly
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global query filters for soft delete
        builder.Entity<BlogPostEntity>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<BlogPostTranslationEntity>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ContentPageEntity>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ContentPageTranslationEntity>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<MediaAssetEntity>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ContactMessageEntity>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var currentUserId = UserContext.CurrentUserId;
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = now;
                    entry.Entity.CreatedByUserId = currentUserId;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAtUtc = now;
                    entry.Entity.UpdatedByUserId = currentUserId;
                    break;

                case EntityState.Deleted:
                    // Soft delete logic
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAtUtc = now;
                    entry.Entity.DeletedByUserId = currentUserId;
                    break;
            }
        }
    }
}
