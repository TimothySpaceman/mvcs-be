using Lib.Modules.Storages.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Storages.Configurations;

public class StorageAccessConfiguration : IEntityTypeConfiguration<StorageAccess>
{
    public void Configure(EntityTypeBuilder<StorageAccess> builder)
    {
        builder.ToTable("storage_access");

        builder.HasKey(a => new { a.StorageId, a.UserId });

        builder.Property(a => a.AccessType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired();

        builder.HasIndex(a => a.UserId);
    }
}