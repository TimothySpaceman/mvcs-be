using Core.Identities;
using Lib.Modules.Vcs.Entities;

namespace Lib.Modules.Vcs.Mappings;

public static class UserIdentityMapping
{
    public static UserIdentityEntity ToEntity(this UserIdentity domain)
    {
        return UserIdentityEntity.Create(domain.Id, domain.Name, domain.Email);
    }

    public static UserIdentity ToDomain(this UserIdentityEntity entity)
    {
        return new UserIdentity(
            entity.Id,
            entity.Name,
            entity.Email
        );
    }
}