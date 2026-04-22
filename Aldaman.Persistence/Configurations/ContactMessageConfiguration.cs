using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessageEntity>
{
    public void Configure(EntityTypeBuilder<ContactMessageEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EmailOrPhone)
            .HasMaxLength(ContactMessageEntity.EmailOrPhoneMaxLength)
            .IsRequired();

        builder.Property(x => x.Subject)
            .HasMaxLength(ContactMessageEntity.SubjectMaxLength)
            .IsRequired();

        builder.Property(x => x.Message)
            .HasMaxLength(ContactMessageEntity.MessageMaxLength)
            .IsRequired();

        builder.Property(x => x.FailureReason)
            .HasMaxLength(ContactMessageEntity.FailureReasonMaxLength);

        builder.Property(x => x.ClientIp)
            .HasMaxLength(ContactMessageEntity.ClientIpMaxLength);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(ContactMessageEntity.UserAgentMaxLength);

        builder.Property(x => x.State)
            .HasConversion<string>()
            .HasMaxLength(ContactMessageEntity.StateMaxLength);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();
    }
}
