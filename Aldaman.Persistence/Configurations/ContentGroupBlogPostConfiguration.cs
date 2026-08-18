using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class ContentGroupBlogPostConfiguration : IEntityTypeConfiguration<ContentGroupBlogPostEntity>
{
    public void Configure(EntityTypeBuilder<ContentGroupBlogPostEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Order)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Unique constraint for ContentGroup + BlogPost
        builder.HasIndex(x => new { x.ContentGroupId, x.BlogPostId })
            .IsUnique();

        // Relationships
        builder.HasOne(x => x.ContentGroup)
            .WithMany(x => x.BlogPosts)
            .HasForeignKey(x => x.ContentGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BlogPost)
            .WithMany(x => x.ContentGroups)
            .HasForeignKey(x => x.BlogPostId)
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
    }
}
