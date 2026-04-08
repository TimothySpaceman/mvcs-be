using System.Text.Json;
using System.Text.Json.Serialization;
using Lib.Modules.Storages.Entities;
using Lib.Modules.Storages.Entities.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lib.Modules.Storages.Configurations;

public class StorageTypeConfiguration : IEntityTypeConfiguration<StorageType>
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters =
        {
            new SchemaFieldJsonConverter(),
            new JsonStringEnumConverter()
        }
    };

    public void Configure(EntityTypeBuilder<StorageType> builder)
    {
        builder.ToTable("storage_types");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Key)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(t => t.Label)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(t => t.ConfigSchema)
            .IsRequired()
            .HasColumnType("jsonb")
            .HasConversion(
                schema => JsonSerializer.Serialize(schema, SerializerOptions),
                json => JsonSerializer.Deserialize<StorageConfigSchema>(json, SerializerOptions)
                        ?? StorageConfigSchema.Empty
            );

        builder.HasIndex(t => t.Key).IsUnique();
        builder.HasIndex(t => t.Label);
    }
}