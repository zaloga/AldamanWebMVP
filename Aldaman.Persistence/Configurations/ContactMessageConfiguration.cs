using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessageEntity>
{
    public void Configure(EntityTypeBuilder<ContactMessageEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.LanguageCode)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasMaxLength(32);

        builder.Property(x => x.Subject)
            .HasMaxLength(256);

        builder.Property(x => x.Message)
            .IsRequired();

        builder.Property(x => x.FailureReason)
            .HasMaxLength(1024);

        builder.Property(x => x.ClientIp)
            .HasMaxLength(64);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(1024);

        builder.Property(x => x.State)
            .HasConversion<string>()
            .HasMaxLength(16);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();
    }
}
