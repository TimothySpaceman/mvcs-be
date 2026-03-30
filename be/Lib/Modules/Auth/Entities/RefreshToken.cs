namespace Lib.Modules.Auth.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    public Session Session { get; private set; } = null!;

    private RefreshToken()
    {
    }

    public static RefreshToken Create(
        Guid sessionId,
        string tokenHash,
        DateTimeOffset expiresAt
    )
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            TokenHash = tokenHash,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = expiresAt
        };
    }

    public bool IsExpired => ExpiresAt < DateTimeOffset.UtcNow;
}