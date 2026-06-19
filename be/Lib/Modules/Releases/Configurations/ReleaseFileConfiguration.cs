using Lib.Modules.Releases.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Releases.Configurations;

public class ReleaseFileConfiguration : IEntityTypeConfiguration<ReleaseFile>
{
    public void Configure(EntityTypeBuilder<ReleaseFile> builder)
    {
        builder.ToTable("release_files");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.FileName)
            .IsRequired()
            .HasMaxLength(512);
        
        builder.Property(r => r.BlobId)
            .HasMaxLength(32);;
        
        builder.Property(r=> r.ReleaseId);
        
        builder.Property(r=> r.CreatedAt)
            .IsRequired();
        
        builder.HasIndex(r=> r.ReleaseId);
    }
}