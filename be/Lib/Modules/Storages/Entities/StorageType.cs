using Lib.Modules.Storages.Entities.Schema;

namespace Lib.Modules.Storages.Entities;

public class StorageType
{
    public Guid Id { get; private set; }
    public string Key { get; private set; }
    public string Label { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public StorageConfigSchema ConfigSchema { get; private set; } = StorageConfigSchema.Empty;

    private StorageType()
    {
    }

    public static StorageType Create(string key, string label, string description, StorageConfigSchema configSchema)
    {
        return new StorageType
        {
            Id = Guid.NewGuid(),
            Key = key,
            Label = label,
            Description = description,
            ConfigSchema = configSchema
        };
    }

    public void Update(string label, string description, StorageConfigSchema configSchema)
    {
        Label = label;
        Description = description;
        ConfigSchema = configSchema;
    }
}