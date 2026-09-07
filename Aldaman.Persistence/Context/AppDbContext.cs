using Aldaman.Persistence.Entities;
using Aldaman.Persistence.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aldaman.Persistence.Context;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public DbSet<ContentEntity> Contents { get; set; } = null!;
    public DbSet<ContentTranslationEntity> ContentTranslations { get; set; } = null!;
    public DbSet<ContentGroupEntity> ContentGroups { get; set; } = null!;
    public DbSet<ContentGroupTranslationEntity> ContentGroupTranslations { get; set; } = null!;
    public DbSet<ContentGroupContentEntity> ContentGroupContents { get; set; } = null!;
    public DbSet<MediaAssetEntity> MediaAssets { get; set; } = null!;
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
        builder.Entity<ContentEntity>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ContentGroupEntity>().HasQueryFilter(e => !e.IsDeleted);
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

        // 1. Creation auditing for all creatable entities
        var entriesCreatable = ChangeTracker.Entries<BaseEntityCreatable>();
        foreach (var entry in entriesCreatable)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = now;
                entry.Entity.CreatedByUserId = currentUserId;
            }
        }

        // 2. Update auditing for all auditable entities
        var entriesAuditable = ChangeTracker.Entries<BaseEntityAuditable>();
        foreach (var entry in entriesAuditable)
        {
            if (entry.State == EntityState.Modified)
            {
                var isSoftDeletingOrRestoring = entry.Entity is BaseEntityAuditableSoftDel &&
                                                entry.Property(nameof(BaseEntityAuditableSoftDel.IsDeleted)).IsModified;

                if (!isSoftDeletingOrRestoring)
                {
                    entry.Entity.UpdatedAtUtc = now;
                    entry.Entity.UpdatedByUserId = currentUserId;
                }
            }
        }

        // 3. Soft deletion handling for auditable soft-deletable entities
        var entriesAuditableSoftDel = ChangeTracker.Entries<BaseEntityAuditableSoftDel>();
        foreach (var entry in entriesAuditableSoftDel)
        {
            var isSoftRestoring = entry.State == EntityState.Modified &&
                                  entry.Property(e => e.IsDeleted).IsModified &&
                                  !entry.Entity.IsDeleted;

            if (entry.State == EntityState.Added)
            {
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

        // 4. Soft deletion handling for creatable soft-deletable entities
        var entriesCreatableSoftDel = ChangeTracker.Entries<BaseEntityCreatableSoftDel>();
        foreach (var entry in entriesCreatableSoftDel)
        {
            var isSoftRestoring = entry.State == EntityState.Modified &&
                                  entry.Property(e => e.IsDeleted).IsModified &&
                                  !entry.Entity.IsDeleted;

            if (entry.State == EntityState.Added)
            {
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
    }
}
