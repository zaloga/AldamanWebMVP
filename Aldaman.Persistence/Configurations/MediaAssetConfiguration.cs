using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAssetEntity>
{
    public void Configure(EntityTypeBuilder<MediaAssetEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OriginalFileName)
            .HasMaxLength(MediaAssetEntity.OriginalFileNameMaxLength)
            .IsRequired();

        builder.Property(x => x.StoredFileName)
            .HasMaxLength(MediaAssetEntity.StoredFileNameMaxLength)
            .IsRequired();

        builder.Property(x => x.RelativePath)
            .HasMaxLength(MediaAssetEntity.RelativePathMaxLength)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(MediaAssetEntity.ContentTypeMaxLength)
            .IsRequired();

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.AltText)
            .HasMaxLength(MediaAssetEntity.AltTextMaxLength);

        builder.Property(x => x.Title)
            .HasMaxLength(MediaAssetEntity.TitleMaxLength);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.IsImage)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.IsVideo)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
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
        builder.HasIndex(x => x.CreatedByUserId);
        builder.HasIndex(x => x.StoredFileName).IsUnique();
    }
}
