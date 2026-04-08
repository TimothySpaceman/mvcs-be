using Lib.Modules.Storages.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Storages.Configurations;

public class StorageConfiguration : IEntityTypeConfiguration<Storage>
{
    public void Configure(EntityTypeBuilder<Storage> builder)
    {
        builder.ToTable("storages");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(s => s.StorageTypeId)
            .IsRequired();

        builder.Property(s => s.Config)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(s => s.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        builder.HasOne(s => s.StorageType)
            .WithMany()
            .HasForeignKey(s => s.StorageTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.AccessEntries)
            .WithOne(a => a.Storage)
            .HasForeignKey(a => a.StorageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}