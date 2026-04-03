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
            .HasMaxLength(5)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.SeoTitle)
            .HasMaxLength(256);

        builder.Property(x => x.SeoDescription)
            .HasMaxLength(512);

        builder.Property(x => x.SeoKeywords)
            .HasMaxLength(512);

        builder.Property(x => x.SectionsJson)
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
    }
}
