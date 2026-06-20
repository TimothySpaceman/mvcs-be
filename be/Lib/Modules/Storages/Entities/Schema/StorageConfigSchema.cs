namespace Lib.Modules.Storages.Entities.Schema;

public class StorageConfigSchema
{
    public IReadOnlyList<SchemaField> Fields { get; init; } = [];

    public static StorageConfigSchema Empty => new();
}