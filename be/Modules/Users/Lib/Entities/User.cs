namespace Users.Lib.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public bool IsEmailVerified { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    
    public UserAvatar? Avatar { get; private set; }
    
    private User(){}

    public static User Create(string username, string displayName, string email)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            DisplayName = displayName,
            Email = email.ToLowerInvariant(),
            IsEmailVerified = false,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }
    
    public bool IsDeleted => DeletedAt.HasValue;
    
    public void VerifyEmail() 
    {
        IsEmailVerified = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateProfile(string displayName)
    {
        DisplayName = displayName;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Delete()
    {
        DeletedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}