using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public abstract class BaseEntityCreatableConfiguration<TEntity> : BaseEntityConfiguration<TEntity>
    where TEntity : BaseEntityCreatable
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.CreatedByUserId);

        // Audit Relationships
        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
