using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPostEntity>
{
    public void Configure(EntityTypeBuilder<BlogPostEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsPublished)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        builder.Property(x => x.CreatedByUserId);

        builder.Property(x => x.UpdatedByUserId);

        builder.Property(x => x.DeletedByUserId);

        // Relationships
        builder.HasOne(x => x.CoverMediaAsset)
            .WithMany()
            .HasForeignKey(x => x.CoverMediaAssetId)
            .OnDelete(DeleteBehavior.Restrict);

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
        builder.HasIndex(x => x.CreatedByUserId);
        builder.HasIndex(x => x.UpdatedByUserId);
    }
}
