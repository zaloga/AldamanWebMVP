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
            .HasMaxLength(ContactMessageEntity.LanguageCodeMaxLength)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(ContactMessageEntity.NameMaxLength)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(ContactMessageEntity.EmailMaxLength)
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasMaxLength(ContactMessageEntity.PhoneMaxLength);

        builder.Property(x => x.Subject)
            .HasMaxLength(ContactMessageEntity.SubjectMaxLength);

        builder.Property(x => x.Message)
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
