using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class PageContentConfiguration : IEntityTypeConfiguration<PageContentEntity>
{
    public void Configure(EntityTypeBuilder<PageContentEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CultureCode)
            .HasMaxLength(PageContentEntity.CultureCodeMaxLength)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(PageContentEntity.TitleMaxLength)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(PageContentEntity.SlugMaxLength)
            .IsRequired();


        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.IsPublished)
            .IsRequired()
            .HasDefaultValue(false);

        // Unique constraint for Page + Language
        builder.HasIndex(x => new { x.PageDefinitionId, x.CultureCode })
            .IsUnique();

        // Navigation: Many PageContents -> One PageDefinition
        builder.HasOne(x => x.PageDefinition)
            .WithMany(x => x.Contents)
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
