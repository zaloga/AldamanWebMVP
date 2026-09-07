using Aldaman.Persistence.Entities;
using Aldaman.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class ContentGroupConfiguration : BaseEntityAuditableSoftDelConfiguration<ContentGroupEntity>
{
    public override void Configure(EntityTypeBuilder<ContentGroupEntity> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.PlaceToShow)
            .IsRequired()
            .HasDefaultValue(PlaceToShowEnum.None);

        builder.Property(x => x.GroupOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Relationship: One ContentGroup -> Many Translations
        builder.HasMany(x => x.Translations)
            .WithOne(x => x.ContentGroup)
            .HasForeignKey(x => x.ContentGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
