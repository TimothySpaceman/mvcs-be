namespace Lib.Modules.Vcs.Entities;

public class UserIdentityEntity
{
    public Guid? Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Email { get; private set; }

    private UserIdentityEntity()
    {
    }

    public static UserIdentityEntity Create(Guid? id, string name, string? email)
    {
        return new UserIdentityEntity
        {
            Id = id,
            Name = name,
            Email = email
        };
    }
}