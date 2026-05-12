using Lib.Modules.Vcs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Vcs.Configurations;

public class RefEntityConfiguration : IEntityTypeConfiguration<RefEntity>
{
    public void Configure(EntityTypeBuilder<RefEntity> builder)
    {
        builder.ToTable("refs");

        builder.HasKey(r => new { r.ProjectId, r.Name });

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.CommitId)
            .IsRequired(false);
    }
}