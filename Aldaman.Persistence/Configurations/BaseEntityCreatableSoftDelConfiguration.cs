using Aldaman.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aldaman.Persistence.Configurations;

public abstract class BaseEntityCreatableSoftDelConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntityCreatableSoftDel
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.CreatedByUserId);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAtUtc);

        builder.Property(x => x.DeletedByUserId);

        // Audit Relationships
        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DeletedByUser)
            .WithMany()
            .HasForeignKey(x => x.DeletedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
