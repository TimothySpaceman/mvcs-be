using Lib.Modules.Vcs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Vcs.Configurations;

public class MergeRequestConfiguration : IEntityTypeConfiguration<MergeRequest>
{
    public void Configure(EntityTypeBuilder<MergeRequest> builder)
    {
        builder.ToTable("merge_requests");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .IsRequired();

        builder.Property(m => m.AuthorId)
            .IsRequired();

        builder.Property(m => m.ProjectId)
            .IsRequired();

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(4096);

        builder.Property(r => r.TargetRefName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.SourceRefName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.MergeCommitId)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.HasIndex(c => c.ProjectId);
    }
}