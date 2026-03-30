using Lib.Modules.Users.DTOs;

namespace Lib.Modules.Auth.Services;

public interface IJwtService
{
    public string GenerateAccessToken(UserDto user);
    public string GenerateRefreshToken(UserDto user);
}