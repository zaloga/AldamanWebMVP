using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class ContentConfiguration : IEntityTypeConfiguration<ContentEntity>
{
    public void Configure(EntityTypeBuilder<ContentEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasDefaultValue(Aldaman.Persistence.Enums.ContentTypeEnum.Post);

        builder.Property(x => x.PlaceToShow)
            .IsRequired();

        builder.Property(x => x.Order)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.IsPublished)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        builder.Property(x => x.CreatedByUserId);

        builder.Property(x => x.UpdatedByUserId);

        builder.Property(x => x.DeletedByUserId);

        // Relationships: One Content -> Many Translations
        builder.HasMany(x => x.Translations)
            .WithOne(x => x.Content)
            .HasForeignKey(x => x.ContentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationships: One Content -> Many ContentGroupContents
        builder.HasMany(x => x.ContentGroups)
            .WithOne(x => x.Content)
            .HasForeignKey(x => x.ContentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationships: Media Asset Cover Image
        builder.HasOne(x => x.CoverMediaAsset)
            .WithMany()
            .HasForeignKey(x => x.CoverMediaAssetId)
            .OnDelete(DeleteBehavior.Restrict);

        // Audit Relationships
        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DeletedByUser)
            .WithMany()
            .HasForeignKey(x => x.DeletedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.IsPublished);
        builder.HasIndex(x => x.PublishedAtUtc);
        builder.HasIndex(x => x.PlaceToShow);
        builder.HasIndex(x => x.Order);
        builder.HasIndex(x => x.CreatedByUserId);
        builder.HasIndex(x => x.UpdatedByUserId);
    }
}
