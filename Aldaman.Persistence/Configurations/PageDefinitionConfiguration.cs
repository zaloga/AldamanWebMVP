using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class PageDefinitionConfiguration : IEntityTypeConfiguration<PageDefinitionEntity>
{
    public void Configure(EntityTypeBuilder<PageDefinitionEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PageKey)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(x => x.PageKey)
            .IsUnique();

        builder.Property(x => x.RouteSegment)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.IsHomePage)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.DefaultSortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Relationship: One PageDefinition -> Many PageContents
        builder.HasMany(x => x.Contents)
            .WithOne(x => x.PageDefinition)
            .HasForeignKey(x => x.PageDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
