using System.Collections.Immutable;
using System.Text.Json;
using Core.FileChanges;
using Lib.Modules.Vcs.Converters;
using Lib.Modules.Vcs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Vcs.Configurations;

public class CommitEntityConfiguration : IEntityTypeConfiguration<CommitEntity>
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters =
        {
            new HashIdJsonConverter()
        }
    };

    public void Configure(EntityTypeBuilder<CommitEntity> builder)
    {
        builder.ToTable("commits");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(c => c.ParentId)
            .HasMaxLength(16);

        builder.Property(c => c.Message)
            .IsRequired()
            .HasMaxLength(4096);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.OwnsOne(c => c.Author, authorBuilder =>
        {
            authorBuilder.Property(a => a.Id).HasColumnName("AuthorId");

            authorBuilder.Property(a => a.Name)
                .HasColumnName("AuthorName")
                .IsRequired()
                .HasMaxLength(256);

            authorBuilder.Property(a => a.Email)
                .HasColumnName("AuthorEmail")
                .HasMaxLength(512);
        });

        builder.Property(с => с.Changes)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, SerializerOptions),
                v => JsonSerializer.Deserialize<ImmutableArray<FileChange>>(v, SerializerOptions)!
            );

        builder.HasIndex(c => c.ProjectId);
    }
}