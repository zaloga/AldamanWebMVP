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
            .HasMaxLength(PageDefinitionEntity.PageKeyMaxLength)
            .IsRequired();

        builder.HasIndex(x => x.PageKey)
            .IsUnique();

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
    }
}
