using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class MediaAssetConfiguration : BaseEntityAuditableSoftDelConfiguration<MediaAssetEntity>
{
    public override void Configure(EntityTypeBuilder<MediaAssetEntity> builder)
    {
        base.Configure(builder);

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

        builder.Property(x => x.AltTextDefault)
            .HasMaxLength(MediaAssetEntity.AltTextDefaultMaxLength);

        builder.Property(x => x.TitleDefault)
            .HasMaxLength(MediaAssetEntity.TitleDefaultMaxLength);

        builder.Property(x => x.IsImage)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.IsVideo)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(x => x.StoredFileName).IsUnique();
    }
}
