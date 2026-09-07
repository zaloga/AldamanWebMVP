using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class ContentGroupTranslationConfiguration : BaseEntityAuditableConfiguration<ContentGroupTranslationEntity>
{
    public override void Configure(EntityTypeBuilder<ContentGroupTranslationEntity> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.CultureCode)
            .HasMaxLength(ContentGroupTranslationEntity.CultureCodeMaxLength)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(ContentGroupTranslationEntity.TitleMaxLength)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(ContentGroupTranslationEntity.SlugMaxLength)
            .IsRequired();

        // Unique constraint for Group + Language
        builder.HasIndex(x => new { x.ContentGroupId, x.CultureCode })
            .IsUnique();
    }
}
