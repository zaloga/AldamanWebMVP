using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class ContentGroupTranslationConfiguration : IEntityTypeConfiguration<ContentGroupTranslationEntity>
{
    public void Configure(EntityTypeBuilder<ContentGroupTranslationEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CultureCode)
            .HasMaxLength(ContentGroupTranslationEntity.CultureCodeMaxLength)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(ContentGroupTranslationEntity.TitleMaxLength)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(ContentGroupTranslationEntity.SlugMaxLength)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Unique constraint for Group + Language
        builder.HasIndex(x => new { x.ContentGroupId, x.CultureCode })
            .IsUnique();

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
