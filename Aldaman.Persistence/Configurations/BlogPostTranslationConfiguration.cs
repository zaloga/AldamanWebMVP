using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class BlogPostTranslationConfiguration : IEntityTypeConfiguration<BlogPostTranslationEntity>
{
    public void Configure(EntityTypeBuilder<BlogPostTranslationEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CultureCode)
            .HasMaxLength(BlogPostTranslationEntity.CultureCodeMaxLength)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(BlogPostTranslationEntity.TitleMaxLength)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(BlogPostTranslationEntity.SlugMaxLength)
            .IsRequired();

        builder.Property(x => x.Perex)
            .HasMaxLength(BlogPostTranslationEntity.PerexMaxLength);



        builder.Property(x => x.PlainText)
            .HasMaxLength(BlogPostTranslationEntity.PlainTextMaxLength);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        builder.Property(x => x.CreatedByUserId);

        builder.Property(x => x.UpdatedByUserId);

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

        // Relationships
        builder.HasOne(x => x.BlogPost)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.BlogPostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => new { x.BlogPostId, x.CultureCode }).IsUnique();
        builder.HasIndex(x => x.Slug).IsUnique();
    }
}
