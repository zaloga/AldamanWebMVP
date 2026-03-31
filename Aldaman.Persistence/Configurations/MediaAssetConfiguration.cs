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
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.StoredFileName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.RelativePath)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.AltText)
            .HasMaxLength(512);

        builder.Property(x => x.Title)
            .HasMaxLength(512);

        builder.Property(x => x.UploadedAtUtc)
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
        builder.HasOne(x => x.UploadedByUser)
            .WithMany() // Assuming we don't need a collection of MediaAssets on AppUser for now
            .HasForeignKey(x => x.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.UploadedByUserId);
        builder.HasIndex(x => x.StoredFileName).IsUnique();
    }
}
