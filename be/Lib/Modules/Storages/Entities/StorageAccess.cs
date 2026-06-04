namespace Lib.Modules.Storages.Entities;

public enum StorageAccessType
{
    ReadWrite = 1,
    Owner = 2,
}

public class StorageAccess
{
    public Guid StorageId { get; private set; }
    public Guid UserId { get; private set; }
    public StorageAccessType AccessType { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public Storage Storage { get; private set; } = null!;

    private StorageAccess()
    {
    }

    public static StorageAccess Create(Guid storageId, Guid userId, StorageAccessType accessType)
    {
        return new StorageAccess
        {
            StorageId = storageId,
            UserId = userId,
            AccessType = accessType,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public void ChangeAccessType(StorageAccessType accessType)
    {
        AccessType = accessType;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public bool IsOwner => AccessType is StorageAccessType.Owner;
    public bool CanWrite => IsOwner || AccessType is StorageAccessType.ReadWrite;
}