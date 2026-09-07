using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public sealed class StyleSettingConfiguration : BaseEntityAuditableSoftDelConfiguration<StyleSettingEntity>
{
    public override void Configure(EntityTypeBuilder<StyleSettingEntity> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Key)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(e => e.Value)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(e => e.DefaultValue)
            .IsRequired()
            .HasMaxLength(512);

        builder.HasIndex(e => e.Key)
            .IsUnique();
    }
}
