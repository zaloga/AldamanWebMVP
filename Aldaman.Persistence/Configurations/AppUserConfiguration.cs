using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(u => u.DisplayName)
            .HasMaxLength(AppUser.DisplayNameMaxLength)
            .IsRequired();

        builder.Property(u => u.CreatedAtUtc)
            .IsRequired();

    }
}
