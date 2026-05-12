using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public sealed class StyleSettingConfiguration : IEntityTypeConfiguration<StyleSettingEntity>
{
    public void Configure(EntityTypeBuilder<StyleSettingEntity> builder)
    {
        builder.ToTable("StyleSettings");
        
        builder.HasKey(e => e.Id);

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
