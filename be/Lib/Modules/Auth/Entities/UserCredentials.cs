using Lib.Modules.Users.Entities;

namespace Lib.Modules.Auth.Entities;

public class UserCredentials
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string PasswordHash { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public User User { get; private set; } = null!;
    
    private UserCredentials()
    {
    }

    public static UserCredentials Create(Guid userId, string passwordHash)
    {
        return new UserCredentials
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PasswordHash = passwordHash,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }
    
    public void UpdatePassword(string newHash)
    {
        PasswordHash = newHash;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}