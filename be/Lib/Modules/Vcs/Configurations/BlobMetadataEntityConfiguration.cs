using Lib.Modules.Vcs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Vcs.Configurations;

public class BlobMetadataEntityConfiguration : IEntityTypeConfiguration<BlobMetadataEntity>
{
    public void Configure(EntityTypeBuilder<BlobMetadataEntity> builder)
    {
        builder.ToTable("blob_metadata");

        builder.HasKey(b => new { b.Id, b.ProjectId });

        builder.Property(b => b.Id)
            .IsRequired();

        builder.HasIndex(b => b.ProjectId);
    }
}