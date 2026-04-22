using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class ContentPageConfiguration : IEntityTypeConfiguration<ContentPageEntity>
{
    public void Configure(EntityTypeBuilder<ContentPageEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PageKey)
            .HasMaxLength(ContentPageEntity.PageKeyMaxLength)
            .IsRequired();

        builder.HasIndex(x => x.PageKey)
            .IsUnique();

        builder.Property(x => x.ShowOnHomePage)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.OrderOnHomePage)
            .IsRequired()
            .HasDefaultValue(0);


        builder.Property(x => x.OrderInNavigation)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Relationship: One ContentPage -> Many ContentPageTranslations
        builder.HasMany(x => x.Translations)
            .WithOne(x => x.ContentPage)
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
