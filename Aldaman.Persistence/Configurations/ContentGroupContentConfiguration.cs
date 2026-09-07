using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class ContentGroupContentConfiguration : BaseEntityAuditableConfiguration<ContentGroupContentEntity>
{
    public override void Configure(EntityTypeBuilder<ContentGroupContentEntity> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Order)
            .IsRequired()
            .HasDefaultValue(0);

        // Relationships
        builder.HasOne(x => x.ContentGroup)
            .WithMany(x => x.Contents)
            .HasForeignKey(x => x.ContentGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Content)
            .WithMany(x => x.ContentGroups)
            .HasForeignKey(x => x.ContentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => new { x.ContentGroupId, x.ContentId }).IsUnique();
    }
}
