namespace Lib.Modules.Storages.Entities;

public class Storage
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Guid StorageTypeId { get; private set; }
    public string Config { get; private set; } = null!; 
    public bool IsPublic { get; private set; } 
    public bool IsDefault { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
 
    public StorageType StorageType { get; private set; } = null!;
    public IReadOnlyCollection<StorageAccess> AccessEntries => _accessEntries.AsReadOnly();
    private readonly List<StorageAccess> _accessEntries = [];
 
    private Storage() { }
 
    public static Storage Create(string name, Guid storageTypeId, string config)
    {
        return new Storage
        {
            Id = Guid.NewGuid(),
            Name = name,
            StorageTypeId = storageTypeId,
            Config = config,
            IsPublic = false,
            IsDefault = false,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }
 
    public void Rename(string name)
    {
        Name = name;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
 
    public void UpdateConfig(string config)
    {
        Config = config;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
