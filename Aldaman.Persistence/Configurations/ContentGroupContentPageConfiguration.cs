using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class ContentGroupContentPageConfiguration : IEntityTypeConfiguration<ContentGroupContentPageEntity>
{
    public void Configure(EntityTypeBuilder<ContentGroupContentPageEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Order)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Unique constraint for ContentGroup + ContentPage
        builder.HasIndex(x => new { x.ContentGroupId, x.ContentPageId })
            .IsUnique();

        // Relationships
        builder.HasOne(x => x.ContentGroup)
            .WithMany(x => x.ContentPages)
            .HasForeignKey(x => x.ContentGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ContentPage)
            .WithMany(x => x.ContentGroups)
            .HasForeignKey(x => x.ContentPageId)
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
    }
}
