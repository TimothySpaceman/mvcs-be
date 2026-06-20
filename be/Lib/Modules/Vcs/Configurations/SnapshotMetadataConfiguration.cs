using System.Text.Json;
using Lib.Modules.Vcs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Vcs.Configurations;

public class SnapshotMetadataConfiguration : IEntityTypeConfiguration<SnapshotMetadata>
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public void Configure(EntityTypeBuilder<SnapshotMetadata> builder)
    {
        builder.ToTable("snapshot_metadata");

        builder.HasKey(m => new { m.CommitId, m.ProjectId });

        builder.Property(m => m.CommitId)
            .IsRequired();

        builder.Property(m => m.SubmittedAt)
            .IsRequired();

        builder.Property(m => m.Data)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, SerializerOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, string[]>>(v, SerializerOptions)!
            );

        builder.HasIndex(m => m.ProjectId);
        builder.HasIndex(m => m.Data).HasMethod("gin");
    }
}