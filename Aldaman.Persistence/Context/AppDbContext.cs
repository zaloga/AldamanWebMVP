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
    public DbSet<StyleSettingEntity> StyleSettings { get; set; } = null!;

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
        builder.Entity<ContentPageEntity>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<MediaAssetEntity>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ContactMessageEntity>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<StyleSettingEntity>().HasQueryFilter(e => !e.IsDeleted);
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
        var currentUserId = UserContext.CurrentUserId;
        var now = DateTime.UtcNow;

        var entriesAuditableSoftDeletable = ChangeTracker.Entries<BaseEntityAuditableSoftDel>();
        foreach (var entry in entriesAuditableSoftDeletable)
        {
            var isSoftDeleting = entry.State == EntityState.Modified &&
                                 entry.Property(e => e.IsDeleted).IsModified &&
                                 entry.Entity.IsDeleted;

            var isSoftRestoring = entry.State == EntityState.Modified &&
                                  entry.Property(e => e.IsDeleted).IsModified &&
                                  !entry.Entity.IsDeleted;

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = now;
                    entry.Entity.CreatedByUserId = currentUserId;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    if (!isSoftDeleting && !isSoftRestoring)
                    {
                        entry.Entity.UpdatedAtUtc = now;
                        entry.Entity.UpdatedByUserId = currentUserId;
                    }
                    break;
            }

            if (entry.Entity.IsDeleted)
            {
                entry.Entity.DeletedAtUtc = now;
                entry.Entity.DeletedByUserId = currentUserId;
            }
            else if (isSoftRestoring)
            {
                entry.Entity.DeletedAtUtc = null;
                entry.Entity.DeletedByUserId = null;
            }
        }

        var entriesCreatableSoftDeletable = ChangeTracker.Entries<BaseEntityCreatableSoftDel>();
        foreach (var entry in entriesCreatableSoftDeletable)
        {
            var isSoftRestoring = entry.State == EntityState.Modified &&
                                  entry.Property(e => e.IsDeleted).IsModified &&
                                  !entry.Entity.IsDeleted;

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = now;
                entry.Entity.CreatedByUserId = currentUserId;
                entry.Entity.IsDeleted = false;
            }

            if (entry.Entity.IsDeleted)
            {
                entry.Entity.DeletedAtUtc = now;
                entry.Entity.DeletedByUserId = currentUserId;
            }
            else if (isSoftRestoring)
            {
                entry.Entity.DeletedAtUtc = null;
                entry.Entity.DeletedByUserId = null;
            }
        }

        var entriesAuditable = ChangeTracker.Entries<BaseEntityAuditable>();
        foreach (var entry in entriesAuditable)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAtUtc = now;
                    entry.Entity.CreatedByUserId = currentUserId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAtUtc = now;
                    entry.Entity.UpdatedByUserId = currentUserId;
                    break;
            }
        }
    }
}
