using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class ContentTranslationConfiguration : BaseEntityAuditableConfiguration<ContentTranslationEntity>
{
    public override void Configure(EntityTypeBuilder<ContentTranslationEntity> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.CultureCode)
            .HasMaxLength(ContentTranslationEntity.CultureCodeMaxLength)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(ContentTranslationEntity.TitleMaxLength)
            .IsRequired();

        builder.Property(x => x.DisplayTitle)
            .HasDefaultValue(true);

        builder.Property(x => x.Slug)
            .HasMaxLength(ContentTranslationEntity.SlugMaxLength)
            .IsRequired();

        builder.Property(x => x.Perex)
            .HasMaxLength(ContentTranslationEntity.PerexMaxLength);

        builder.Property(x => x.BodyHtml);

        builder.Property(x => x.BodyDeltaJson);

        builder.Property(x => x.PlainText)
            .HasMaxLength(ContentTranslationEntity.PlainTextMaxLength);

        builder.Property(x => x.DisplayExpanded)
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(x => new { x.ContentId, x.CultureCode }).IsUnique();
        builder.HasIndex(x => new { x.CultureCode, x.Slug }).IsUnique();
    }
}
