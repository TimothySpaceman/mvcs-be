using Lib.Modules.Users.Entities;

namespace Lib.Modules.Auth.Entities;

public class Session
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DeviceInfo DeviceInfo { get; private set; } = null!;
    public string IpAddress { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset LastActiveAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }

    public User User { get; private set; } = null!;
    public RefreshToken RefreshToken { get; private set; } = null!;

    private Session()
    {
    }

    public static Session Create(
        Guid userId,
        DeviceInfo deviceInfo,
        string ipAddress
    )
    {
        return new Session
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            CreatedAt = DateTimeOffset.UtcNow,
            LastActiveAt = DateTimeOffset.UtcNow,
        };
    }
    
    public bool IsRevoked => RevokedAt is not null;

    public void Revoke()
    {
        RevokedAt = DateTimeOffset.UtcNow;
    }
    
    public void Refresh()
    {
        LastActiveAt = DateTimeOffset.UtcNow;
    }
}