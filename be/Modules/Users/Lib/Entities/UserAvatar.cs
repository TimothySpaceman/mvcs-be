namespace Users.Lib.Entities;

public class UserAvatar
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string StorageKey { get; private set; } = null!;
    public string Url { get; private set; } = null!;
    public long SizeBytes { get; private set; }
    public string MimeType { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    private UserAvatar() { }

    public static UserAvatar Create(Guid userId, string storageKey, string url, long sizeBytes, string mimeType)
    {
        return new UserAvatar
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            StorageKey = storageKey,
            Url = url,
            SizeBytes = sizeBytes,
            MimeType = mimeType,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}