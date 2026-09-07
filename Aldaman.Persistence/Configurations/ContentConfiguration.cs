using Aldaman.Persistence.Entities;
using Aldaman.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class ContentConfiguration : BaseEntityAuditableSoftDelConfiguration<ContentEntity>
{
    public override void Configure(EntityTypeBuilder<ContentEntity> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasDefaultValue(ContentTypeEnum.Post);

        builder.Property(x => x.PlaceToShow)
            .IsRequired();

        builder.Property(x => x.Order)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.IsPublished)
            .IsRequired()
            .HasDefaultValue(false);

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

        // Indexes
        builder.HasIndex(x => x.IsPublished);
        builder.HasIndex(x => x.PublishedAtUtc);
        builder.HasIndex(x => x.PlaceToShow);
        builder.HasIndex(x => x.Order);
    }
}
