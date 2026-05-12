using Core.Identities;

namespace Lib.Modules.Vcs.DTOs;

public record UserIdentityDto(
    Guid? Id,
    string Name,
    string? Email
)
{
    public static UserIdentityDto FromDomain(UserIdentity domain)
    {
        return new UserIdentityDto(domain.Id, domain.Name, domain.Email);
    }
}