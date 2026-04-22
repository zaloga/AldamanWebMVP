using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class ContentPageTranslationConfiguration : IEntityTypeConfiguration<ContentPageTranslationEntity>
{
    public void Configure(EntityTypeBuilder<ContentPageTranslationEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CultureCode)
            .HasMaxLength(ContentPageTranslationEntity.CultureCodeMaxLength)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(ContentPageTranslationEntity.TitleMaxLength)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(ContentPageTranslationEntity.SlugMaxLength)
            .IsRequired();
            
        builder.Property(x => x.BodyHtml);
        builder.Property(x => x.BodyDeltaJson);
        builder.Property(x => x.PlainText)
            .HasMaxLength(ContentPageTranslationEntity.PlainTextMaxLength);


        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();


        // Unique constraint for Page + Language
        builder.HasIndex(x => new { x.ContentPageId, x.CultureCode })
            .IsUnique();

        // Navigation: Many ContentPageTranslations -> One ContentPage
        builder.HasOne(x => x.ContentPage)
            .WithMany(x => x.Translations)
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

        builder.HasOne(x => x.DeletedByUser)
            .WithMany()
            .HasForeignKey(x => x.DeletedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
